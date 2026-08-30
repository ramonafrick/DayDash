using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Storage;

public static class StorageModuleExtensions
{
    /// <summary>
    /// Registers the shared SQLite context and the generic repository.
    /// The database path is supplied by the host (e.g. the MAUI app via
    /// <c>FileSystem.AppDataDirectory</c>) so this module stays platform-agnostic.
    /// </summary>
    public static IServiceCollection AddDayDashStorage(this IServiceCollection services, string databasePath)
    {
        services.AddDbContext<DayDashDbContext>(options =>
            options.UseSqlite($"Data Source={databasePath}"));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        return services;
    }
}
