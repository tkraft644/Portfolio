using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using Xunit;

namespace Portfolio.Tests;

public class RateLimitingTests : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RateLimitingTests(PortfolioWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task ContactPost_IsRateLimited()
    {
        HttpResponseMessage? response = null;

        for (var i = 0; i < 6; i++)
        {
            response = await _client.PostAsync(
                "/Home/Contact",
                new FormUrlEncodedContent(new Dictionary<string, string>()));
        }

        Assert.NotNull(response);
        Assert.Equal((HttpStatusCode)429, response!.StatusCode);
    }
}
