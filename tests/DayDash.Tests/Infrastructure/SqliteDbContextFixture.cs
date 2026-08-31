using System.Globalization;
using DayDash.Migrations;
using DayDash.Modules.Settings.Application.Services;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;

namespace DayDash.Tests.Infrastructure;

/// <summary>
/// A real SQLite database held entirely in memory via a kept-open connection. Exercises actual
/// relational behaviour (FK cascade, unique indexes, DateOnly/TimeOnly TEXT round-trips) and
/// runs the checked-in migrations, so those are under test on every run.
/// </summary>
public sealed class SqliteDbContextFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    public SqliteDbContextFixture()
    {
        // Seeders resolve default names via IStringLocalizer against CurrentUICulture; pin it so
        // seeded data is deterministic regardless of the host machine's locale.
        var culture = new CultureInfo(SupportedCultures.Default);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<DayDashDbContext>()
            .UseSqlite(_connection, sqlite => sqlite.MigrationsAssembly(DayDashDbContextFactory.MigrationsAssemblyName))
            .Options;

        Context = new DayDashDbContext(options, DayDashModelConfigurations.All);
        Context.Database.Migrate();
    }

    public DayDashDbContext Context { get; }

    public FakeTimeProvider Time { get; } = new(new DateTimeOffset(2026, 3, 10, 8, 0, 0, TimeSpan.Zero));

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }
}
