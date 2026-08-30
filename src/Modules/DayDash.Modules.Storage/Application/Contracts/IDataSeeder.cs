using DayDash.Modules.Storage.Infrastructure;

namespace DayDash.Modules.Storage.Application.Contracts;

/// <summary>
/// A feature module's default-data seeder, run once by <see cref="IDatabaseInitializer"/> after
/// the schema is up to date. Implementations MUST be idempotent: no-op when their own table
/// already has rows.
/// </summary>
public interface IDataSeeder
{
    /// <summary>Relative run order; lower runs first.</summary>
    int Order { get; }

    Task SeedAsync(DayDashDbContext context, CancellationToken ct = default);
}
