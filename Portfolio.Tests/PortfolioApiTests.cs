using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Xunit;

namespace Portfolio.Tests;

public class PortfolioApiTests : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client;

    public PortfolioApiTests(PortfolioWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetSummary_ReturnsExpectedShape()
    {
        using var response = await _client.GetAsync("/api/portfolio/summary");
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());

        Assert.True(doc.RootElement.TryGetProperty("name", out var name));
        Assert.False(string.IsNullOrWhiteSpace(name.GetString()));

        Assert.True(doc.RootElement.TryGetProperty("mainTechnologies", out var tech));
        Assert.Equal(JsonValueKind.Array, tech.ValueKind);
    }
}
