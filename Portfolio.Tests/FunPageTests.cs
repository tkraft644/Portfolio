using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Portfolio.Tests;

public class FunPageTests : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client;

    public FunPageTests(PortfolioWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetFunPage_ReturnsCanvasAndScriptReference()
    {
        using var response = await _client.GetAsync("/Home/Fun");
        response.EnsureSuccessStatusCode();

        var html = await response.Content.ReadAsStringAsync();
        Assert.Contains("id=\"dino-canvas\"", html);
        Assert.Contains("/js/dino.js", html);
    }

    [Fact]
    public async Task GetDinoScript_ReturnsJavaScript()
    {
        using var response = await _client.GetAsync("/js/dino.js");
        response.EnsureSuccessStatusCode();

        var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
        Assert.Contains("javascript", contentType);
    }
}

