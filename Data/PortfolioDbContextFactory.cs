using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Portfolio.Data;

public sealed class PortfolioDbContextFactory : IDesignTimeDbContextFactory<PortfolioDbContext>
{
    public PortfolioDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PortfolioDbContext>();
        var connectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__Portfolio") ??
            "Server=localhost,1433;Database=portfolio;User Id=sa;Password=Your_password123!;TrustServerCertificate=True";

        optionsBuilder.UseSqlServer(connectionString);
        return new PortfolioDbContext(optionsBuilder.Options);
    }
}
