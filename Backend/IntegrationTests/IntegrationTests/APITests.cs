using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IntegrationTests
{
    public class APITests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly CustomWebApplicationFactory<Program> _factory;
        private readonly HttpClient _httpClient;

        public APITests(CustomWebApplicationFactory<Program> factory)
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
            // var response = await _httpClient.GetAsync("/api/posts");
            // var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
            var response = await _httpClient.GetAsync("/api/posts");
            var rawContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Raw Response: {rawContent}");

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var posts = await response.Content.ReadFromJsonAsync<List<Post>>();
            posts.Should().NotBeNull();
            posts.Should().HaveCount(3);
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            posts.Should().NotBeNull();
            posts.Should().HaveCount(3); // Assuming SeedTestDatabaseAsync adds 3 posts
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
