using Application.Abstractions;
using Application.Posts.Queries;
using Application.Posts.QueryHandlers;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Net.Http.Json;

namespace IntegrationTests
{
    public class APITesting : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private HttpClient _httpClient;

        public APITesting(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
                BaseAddress = new Uri("localhost:8080")
            });
        }

        // Intergration test
        [Fact]
        public async Task ReturnsSucess()
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SocialDbcontext>();

                db.Database.Migrate();
                Seeding.InitializeTestDB(db);
            }

            var response = await _httpClient.GetAsync("api/posts/");
            var result = await response.Content.ReadFromJsonAsync<List<Post>>();
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            result.Should().HaveCount(3);
        }
        
        [Fact]
        public async Task CreatePost_ReturnsCreatedResponse()
        {
            
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SocialDbcontext>();

                db.Database.Migrate();
                Seeding.InitializeTestDB(db);
            }
            // Arrange
            var newPost = new Post
            {
                Comments = "This is a test comment",
                Content = "This is test content",
                DateCreated = DateTime.UtcNow,
                LastModified = DateTime.UtcNow
            };

            // Act
            var response = await _httpClient.PostAsJsonAsync("api/posts", newPost);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

            // Ensure the response contains a valid location header
            var createdPost = await response.Content.ReadFromJsonAsync<Post>();
            createdPost.Should().NotBeNull();
            createdPost.Id.Should().BeGreaterThan(0);
            createdPost.Comments.Should().Be(newPost.Comments);
            createdPost.Content.Should().Be(newPost.Content);
        }


        // Unit tests
        [Fact]
        public async Task Handle_ReturnsAllPosts()
        {
            // Arrange
            var mockPostRepository = new Mock<IPostRepository>();
            var expectedPosts = new List<Post>
        {
            new Post { Id = 1, Comments = "Post 1", Content = "Content 1" },
            new Post { Id = 2, Comments = "Post 2", Content = "Content 2" }
        };

            mockPostRepository.Setup(repo => repo.GetPosts()).ReturnsAsync(expectedPosts);

            var handler = new GetAllPostsHandler(mockPostRepository.Object);
            var request = new GetAllPosts();
            var cancellationToken = new CancellationToken();

            // Act
            var result = await handler.Handle(request, cancellationToken);

            // Assert
            Assert.Equal(expectedPosts, result);
        }

    }
}
