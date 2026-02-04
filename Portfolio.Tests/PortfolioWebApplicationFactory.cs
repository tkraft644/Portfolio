using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Portfolio.Tests;

public sealed class PortfolioWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        var projectRoot = FindProjectRoot();
        builder.UseContentRoot(projectRoot);
        builder.UseWebRoot(Path.Combine(projectRoot, "wwwroot"));
    }

    private static string FindProjectRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Portfolio.csproj")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Could not locate Portfolio.csproj from test run.");
    }
}
