using System.Net;
using System.Net.Http.Json;
using System.Text;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Tests.IntegrationTests
{
    public class ApiTests : IClassFixture<CustomWebApplicationFactory<Program>>, IDisposable
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;
        private readonly IServiceScope _scope;
        private readonly SocialDbcontext _dbContext;

        public ApiTests()
        {
            _factory = new CustomWebApplicationFactory<Program>();
            _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
            
            // Create a dedicated scope and db context for each test
            _scope = _factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<SocialDbcontext>();
            
            // Ensure clean database for each test
            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
            SeedTestDatabaseAsync(_dbContext).GetAwaiter().GetResult();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
            _scope?.Dispose();
            _httpClient?.Dispose();
        }

        [Fact]
        public async Task GetAllPosts_ReturnsSuccessAndPosts()
        {
            // Act
            var response = await _httpClient.GetAsync("/api/posts");
            var rawContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Raw Response: {rawContent}");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
            posts.Should().NotBeNull();
            posts.Should().HaveCount(3);
        }
        
        [Fact]
        public async Task Delete_Posts_ReturnsSuccessAndPosts()
        {
            // Act
            var response = await _httpClient.DeleteAsync("api/posts/3");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            
            // Verify the post was actually deleted
            var getResponse = await _httpClient.GetAsync("/api/posts");
            var posts = await getResponse.Content.ReadFromJsonAsync<List<Post>>();
            posts.Should().HaveCount(2);
        }
        
        [Fact]
        public async Task Get_Post_By_Id_ReturnsSuccessAndPosts()
        {
            // Act
            var response = await _httpClient.GetAsync("api/post/1");
            var rawContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Raw Response: {rawContent}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var post = await response.Content.ReadFromJsonAsync<Post>();
            post.Should().NotBeNull();
            post.Id.Should().Be(1);
        }
 
        [Fact]
        public async Task Post_Saves_Success()
        {
            // Arrange
            var postPayload = new Post()
            {
                Content = "Test post content",
                Comments = "Test comments",
                DateCreated = DateTime.Now
            };

            var content = new StringContent(JsonConvert.SerializeObject(postPayload), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("api/posts", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            
            // Verify the post was actually created
            var getResponse = await _httpClient.GetAsync("/api/posts");
            var posts = await getResponse.Content.ReadFromJsonAsync<List<Post>>();
            posts.Should().HaveCount(4); // 3 seeded + 1 new
        }
        
        private async Task SeedTestDatabaseAsync(SocialDbcontext db)
        {
            // Clear any existing data and seed fresh
            db.Posts.RemoveRange(db.Posts);
            await db.SaveChangesAsync();

            await db.Posts.AddRangeAsync(
                new Post { Id = 1, Comments = "Comment 1", Content = "Content 1", DateCreated = DateTime.Now },
                new Post { Id = 2, Comments = "Comment 2", Content = "Content 2", DateCreated = DateTime.Now },
                new Post { Id = 3, Comments = "Comment 3", Content = "Content 3", DateCreated = DateTime.Now }
            );
            await db.SaveChangesAsync();
        }
    }
}