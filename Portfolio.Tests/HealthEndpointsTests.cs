using Microsoft.AspNetCore.Mvc.Testing;
using System.Text.Json;
using Xunit;

namespace Portfolio.Tests;

public class HealthEndpointsTests : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthEndpointsTests(PortfolioWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Health_ReturnsHealthy()
    {
        using var response = await _client.GetAsync("/health");
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", doc.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Ready_ReturnsHealthy()
    {
        using var response = await _client.GetAsync("/health/ready");
        response.EnsureSuccessStatusCode();

        using var doc = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", doc.RootElement.GetProperty("status").GetString());
    }
}
