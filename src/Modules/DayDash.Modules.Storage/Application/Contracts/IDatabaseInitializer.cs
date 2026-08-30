namespace DayDash.Modules.Storage.Application.Contracts;

/// <summary>
/// Brings the database up to date at app start: applies migrations (SQLite) or creates the
/// schema (InMemory preview), then runs all registered <see cref="IDataSeeder"/>s in order.
/// Never deletes user data.
/// </summary>
public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken ct = default);
}
