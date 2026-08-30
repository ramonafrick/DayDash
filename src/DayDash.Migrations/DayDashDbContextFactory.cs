using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Migrations;

/// <summary>
/// Builds a <see cref="DayDashDbContext"/> for the non-DI construction sites: the design-time
/// <c>dotnet ef</c> tooling and the Android widgets (which run in a BroadcastReceiver with no
/// service provider).
/// </summary>
public static class DayDashDbContextFactory
{
    public const string MigrationsAssemblyName = "DayDash.Migrations";

    public static DbContextOptions<DayDashDbContext> BuildSqliteOptions(string databasePath, bool readOnly = false)
    {
        var connectionString = readOnly
            ? $"Data Source={databasePath};Mode=ReadOnly"
            : $"Data Source={databasePath}";

        return new DbContextOptionsBuilder<DayDashDbContext>()
            .UseSqlite(connectionString, sqlite => sqlite.MigrationsAssembly(MigrationsAssemblyName))
            .Options;
    }

    public static DayDashDbContext CreateSqlite(string databasePath, bool readOnly = false)
        => new(BuildSqliteOptions(databasePath, readOnly), DayDashModelConfigurations.All);
}
