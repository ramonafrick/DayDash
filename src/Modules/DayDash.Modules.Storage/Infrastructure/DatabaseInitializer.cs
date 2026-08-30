using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;

namespace DayDash.Modules.Storage.Infrastructure;

/// <summary>
/// Applies pending migrations (relational providers) or creates the schema (InMemory), then
/// runs every <see cref="IDataSeeder"/> ordered by <see cref="IDataSeeder.Order"/>.
/// </summary>
public sealed class DatabaseInitializer(
    DayDashDbContext context,
    IEnumerable<IDataSeeder> seeders,
    ILogger<DatabaseInitializer> logger) : IDatabaseInitializer
{
    public async Task InitializeAsync(CancellationToken ct = default)
    {
        if (context.Database.IsRelational())
        {
            await context.Database.MigrateAsync(ct);
        }
        else
        {
            await context.Database.EnsureCreatedAsync(ct);
        }

        foreach (var seeder in seeders.OrderBy(s => s.Order))
        {
            try
            {
                await seeder.SeedAsync(context, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Seeder {Seeder} failed", seeder.GetType().Name);
                throw;
            }
        }
    }
}
