using DayDash.Modules.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Migrations;

/// <summary>
/// SQLite convenience wrapper around <see cref="StorageModuleExtensions.AddDayDashStorage"/>.
/// Lives in the migrations assembly (the only place besides the MAUI head that references the
/// SQLite provider) so it can hard-code the migrations assembly name.
/// </summary>
public static class DayDashSqliteStorageExtensions
{
    public static IServiceCollection AddDayDashSqliteStorage(this IServiceCollection services, string databasePath)
        => services.AddDayDashStorage(options => options.UseSqlite(
            $"Data Source={databasePath}",
            sqlite => sqlite.MigrationsAssembly(DayDashDbContextFactory.MigrationsAssemblyName)));
}
