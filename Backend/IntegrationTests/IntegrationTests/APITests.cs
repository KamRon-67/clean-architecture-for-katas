using System.Net;
using System.Net.Http.Json;
using System.Text;
using Domain.Entities;
using FluentAssertions;
using Infrastructure;
using IntegrationTests;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace Tests.IntegrationTests
{
    public class ApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public ApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _httpClient = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        [Fact]
        public async Task GetAllPosts_ReturnsSuccessAndPosts()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SocialDbcontext>();

                db.Database.EnsureCreated();
                await SeedTestDatabaseAsync(db); // Seed the database with test data
            }

            // Act
            var response = await _httpClient.GetAsync("/api/posts");
            var rawContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Raw Response: {rawContent}");
            
            // Assert
            var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            posts.Should().NotBeNull();
            posts.Should().HaveCount(3); // Assuming SeedTestDatabaseAsync adds 3 posts
        }
        
        [Fact]
        public async Task Delete_Posts_ReturnsSuccessAndPosts()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SocialDbcontext>();

                db.Database.EnsureCreated();
                await SeedTestDatabaseAsync(db); // Seed the database with test data
            }

            // Act
            var response = await _httpClient.DeleteAsync("api/posts/3");
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [Fact]
        public async Task Get_Post_By_Id_ReturnsSuccessAndPosts()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SocialDbcontext>();

                db.Database.EnsureCreated();
                await SeedTestDatabaseAsync(db); // Seed the database with test data
            }

            // Act
            var response = await _httpClient.GetAsync("api/post/1");
            var rawContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Raw Response: {rawContent}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var post = await response.Content.ReadFromJsonAsync<Post>();
            post.Should().NotBeNull();
        }
 
        [Fact]
        public async Task Post_Saves_Success()
        {
            // Arrange
            using (var scope = _factory.Services.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<SocialDbcontext>();

                db.Database.EnsureDeleted(); // Optional but recommended to start clean
                db.Database.EnsureCreated();

                await SeedTestDatabaseAsync(db);
            }
            
            var postPayload = new Post()
            {
                Content = "Test post content",
                Id = 3,
                Comments = "data",
                DateCreated = DateTime.Now
            };

            var content = new StringContent(JsonConvert.SerializeObject(postPayload), Encoding.UTF8, "application/json");

            // Act
            var response = await _httpClient.PostAsync("api/posts", content);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created); // or HttpStatusCode.Created if you return 201
        }
        
        private async Task SeedTestDatabaseAsync(SocialDbcontext db)
        {
            if (!await db.Posts.AnyAsync())
            {
                await db.Posts.AddRangeAsync(
                    new Post { Comments = "Post 1", Content = "Content 1" },
                    new Post { Comments = "Post 2", Content = "Content 2" },
                    new Post { Comments = "Post 3", Content = "Content 3" }
                );
                await db.SaveChangesAsync();
            }
        }
    }
}
