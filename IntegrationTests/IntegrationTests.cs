using Microsoft.AspNetCore.Mvc.Testing;

namespace IntegrationTests;

public class IntegrationTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private CustomWebApplicationFactory<Program> _factory;
    private IHttpClientFactory _httpClientFactory;

    public IntegrationTests(CustomWebApplicationFactory<Program> factory,IHttpClientFactory httpClientFactory)
    {
        _factory = factory;
        _httpClientFactory = _httpClientFactory;
    }

    // This test is working and show
    [Fact]
    public async Task CanRead()
    {
        using HttpClient client2 = _httpClientFactory.CreateClient("name" ?? "");
        //var client = _factory.CreateClient();

        var result = await client2.GetAsync("/api/posts");
        
        Assert.NotNull(result);
    }
}