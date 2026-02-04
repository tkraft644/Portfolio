using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Portfolio.Tests;

public class SecurityHeadersTests : IClassFixture<PortfolioWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SecurityHeadersTests(PortfolioWebApplicationFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task GetRoot_ReturnsExpectedSecurityHeaders()
    {
        using var response = await _client.GetAsync("/");
        response.EnsureSuccessStatusCode();

        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").Single());
        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").Single());
        Assert.Equal("no-referrer", response.Headers.GetValues("Referrer-Policy").Single());
        Assert.Equal(
            "camera=(), microphone=(), geolocation=()",
            response.Headers.GetValues("Permissions-Policy").Single());
        Assert.Equal("same-origin", response.Headers.GetValues("Cross-Origin-Opener-Policy").Single());

        var csp = response.Headers.GetValues("Content-Security-Policy").Single();
        Assert.Contains("default-src 'self'", csp);
        Assert.Contains("script-src 'self'", csp);
        Assert.Contains("style-src 'self' 'unsafe-inline'", csp);
    }
}
