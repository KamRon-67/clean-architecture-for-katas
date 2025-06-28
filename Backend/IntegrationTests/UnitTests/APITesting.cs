using System.Net.Http.Json;
using Application.Abstractions;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Use_Cases.Posts.Queries;
using Use_Cases.Posts.QueryHandlers;

// Make sure Xunit namespace is imported

namespace Tests.UnitTests
{
    // Implement IAsyncLifetime for per-test setup/teardown
    public class ApiTesting : IClassFixture<CustomWebApplicationFactory<Program>>, IAsyncLifetime
    {
        private readonly CustomWebApplicationFactory<Program> _factory; // Use the custom factory
        private HttpClient _httpClient;
        private IServiceScope _scope; // Keep scope for cleanup if needed

        public ApiTesting(CustomWebApplicationFactory<Program> factory) // Inject the custom factory
        {
            _factory = factory;
            // Client can be created here or in InitializeAsync if preferred
            _httpClient = _factory.CreateClient();
        }

        // Runs BEFORE EACH test method because of IAsyncLifetime
        public async Task InitializeAsync()
        {
            // Create a scope for THIS TEST to get the DbContext
            // It's generally better practice to create a scope per operation/test
            // rather than reusing a single scope across tests.
            _scope = _factory.Services.CreateScope(); // Create scope for the test
            var dbContext = _scope.ServiceProvider.GetRequiredService<SocialDbcontext>();

            // Reset the database for the upcoming test
            await dbContext.Database.EnsureDeletedAsync(); // Drop the schema
            await dbContext.Database.EnsureCreatedAsync(); // Create schema based on Model

            // Optional: Perform common seeding here if EVERY test needs the same base data
            // Seeding.InitializeTestDB(dbContext); // Pass the context from the current scope
        }

        // Runs AFTER EACH test method because of IAsyncLifetime
        public Task DisposeAsync()
        {
            // Dispose the scope created in InitializeAsync to release services
            _scope?.Dispose();
            // _httpClient is typically managed by the factory and disposed with it,
            // but explicitly disposing it here is also safe if needed.
            // _httpClient?.Dispose(); // Optional: Dispose client if necessary

            // Note: The DbConnection in CustomWebApplicationFactory remains open
            // because it's a Singleton managed by the factory itself.
            // The database content is cleared by InitializeAsync before the *next* test.
            return Task.CompletedTask;
        }

        // --- Your Tests ---

        [Fact]
        public async Task ReturnsSucess()
        {
            // Optional: Seed data specific to this test if needed
            using (var seedScope = _factory.Services.CreateScope())
            {
                var db = seedScope.ServiceProvider.GetRequiredService<SocialDbcontext>();
                Seeding.InitializeTestDB(db); // Example: Seed 3 posts
            }

            var response = await _httpClient.GetAsync("api/posts/");

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API returned error ({response.StatusCode}): {errorContent}");
                // Optionally add: Assert.Fail($"API Error: {response.StatusCode}");
            }
            response.EnsureSuccessStatusCode(); // Or use FluentAssertions for status

            var result = await response.Content.ReadFromJsonAsync<List<Post>>();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Should().HaveCount(3); // Assuming InitializeTestDB creates 3 posts
        }

        [Fact]
        public async Task CreatePost_ReturnsCreatedResponse()
        {
            // Arrange: No initial data needed for this test
            var newPost = new Post
            {
                Comments = "This is a test comment",
                Content = "This is test content",
                DateCreated = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/posts", newPost);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"API returned error ({response.StatusCode}): {errorContent}");
            }

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

            var createdPost = await response.Content.ReadFromJsonAsync<Post>();
            createdPost.Should().NotBeNull();
            createdPost.Comments.Should().Be(newPost.Comments);
            createdPost.Content.Should().Be(newPost.Content);

            // Optional: Verify it was actually saved in the DB
            // using (var verifyScope = _factory.Services.CreateScope())
            // {
            //     var db = verifyScope.ServiceProvider.GetRequiredService<SocialDbcontext>();
            //     var postInDb = await db.Posts.FindAsync(createdPost.Id);
            //     postInDb.Should().NotBeNull();
            //     postInDb.Content.Should().Be(newPost.Content);
            // }
        }

        // --- Unit Tests (These don't use the factory/DB directly) ---
        [Fact]
        public async Task Handle_ReturnsAllPosts()
        {
            // Arrange
            var mockPostRepository = new Mock<IPostRepository>();
            var expectedPosts = new List<PostDto>
            {
                new PostDto(1, "Post 1", "Content 1"),
                new PostDto(2,"Post 2", "Content 2")
            };
            mockPostRepository.Setup(repo => repo.GetPosts()).ReturnsAsync(expectedPosts);
            var handler = new GetAllPostsHandler(mockPostRepository.Object);
            var request = new GetAllPosts();

            // Act
            var result = await handler.Handle(request); // Pass CancellationToken

            // Assert
            result.Should().BeEquivalentTo(expectedPosts); // Use BeEquivalentTo for collection comparison
        }
        
        [Fact]
        public async Task Handle_ReturnsGetPostByIdHandler()
        {
            // Arrange
            var mockPostRepository = new Mock<IPostRepository>();

            var expectedPost = new PostDto(1, "Post 1", "Content 1");
            
            mockPostRepository.Setup(repo => repo.GetPostById(1)).ReturnsAsync(expectedPost);
            var handler = new GetPostByIdHandler(mockPostRepository.Object);
            var request = new GetPostById()
            {
                PostId = 1
            };

            // Act
            var result = await handler.Handle(request); // Pass CancellationToken

            // Assert
            result.Should().BeEquivalentTo(expectedPost); // Use BeEquivalentTo for collection comparison
        }
    }
}