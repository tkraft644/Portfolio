using System.Data;
using Microsoft.EntityFrameworkCore;

namespace Portfolio.Data;

public sealed class PortfolioDbMigrator
{
    private readonly PortfolioDbContext _db;
    private readonly ILogger<PortfolioDbMigrator> _logger;

    public PortfolioDbMigrator(PortfolioDbContext db, ILogger<PortfolioDbMigrator> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        const int maxAttempts = 10;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                await TryCreateBaselineForExistingDatabaseAsync(cancellationToken);
                await _db.Database.MigrateAsync(cancellationToken);
                _logger.LogInformation("EF Core migrations applied.");
                return;
            }
            catch (Exception ex) when (attempt < maxAttempts)
            {
                _logger.LogWarning(ex, "Database migration not ready yet (attempt {Attempt}/{Max}). Retrying...", attempt, maxAttempts);
                await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
            }
        }
    }

    private async Task TryCreateBaselineForExistingDatabaseAsync(CancellationToken cancellationToken)
    {
        var applied = (await _db.Database.GetAppliedMigrationsAsync(cancellationToken)).ToList();
        if (applied.Count > 0)
        {
            return;
        }

        var pending = (await _db.Database.GetPendingMigrationsAsync(cancellationToken)).ToList();
        if (pending.Count == 0)
        {
            return;
        }

        if (!await HasExistingPortfolioSchemaAsync(cancellationToken))
        {
            return;
        }

        var baselineMigrationId = pending[0];
        const string productVersion = "9.0.0";

        await EnsureMigrationHistoryTableAsync(cancellationToken);

        await _db.Database.ExecuteSqlInterpolatedAsync(
            $"""
             IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = {baselineMigrationId})
             BEGIN
                 INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                 VALUES ({baselineMigrationId}, {productVersion});
             END
             """,
            cancellationToken);

        _logger.LogWarning(
            "Detected existing schema without migration history. Baseline migration '{MigrationId}' was marked as applied.",
            baselineMigrationId);
    }

    private async Task EnsureMigrationHistoryTableAsync(CancellationToken cancellationToken)
    {
        await _db.Database.ExecuteSqlRawAsync(
            """
            IF OBJECT_ID(N'[__EFMigrationsHistory]', N'U') IS NULL
            BEGIN
                CREATE TABLE [__EFMigrationsHistory] (
                    [MigrationId] nvarchar(150) NOT NULL,
                    [ProductVersion] nvarchar(32) NOT NULL,
                    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                );
            END
            """,
            cancellationToken);
    }

    private async Task<bool> HasExistingPortfolioSchemaAsync(CancellationToken cancellationToken)
    {
        const string sql = """
                           SELECT CASE
                               WHEN EXISTS (
                                   SELECT 1
                                   FROM [sys].[tables]
                                   WHERE [name] IN (N'PortfolioProfiles', N'PortfolioProjects', N'PortfolioTechnologies'))
                               THEN CAST(1 AS bit)
                               ELSE CAST(0 AS bit)
                           END;
                           """;

        var connection = _db.Database.GetDbConnection();
        var shouldClose = connection.State != ConnectionState.Open;

        if (shouldClose)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using var command = connection.CreateCommand();
            command.CommandText = sql;
            var scalar = await command.ExecuteScalarAsync(cancellationToken);
            return scalar switch
            {
                bool bit => bit,
                byte value => value == 1,
                short value => value == 1,
                int value => value == 1,
                long value => value == 1,
                _ => false
            };
        }
        finally
        {
            if (shouldClose)
            {
                await connection.CloseAsync();
            }
        }
    }
}
