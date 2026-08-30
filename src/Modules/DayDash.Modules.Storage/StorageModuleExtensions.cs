using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Storage;

public static class StorageModuleExtensions
{
    /// <summary>
    /// Registers the shared <see cref="DayDashDbContext"/> plus the generic repository, the
    /// database initializer and the data-change notifier. The concrete EF Core provider is
    /// supplied by the host via <paramref name="configureOptions"/> so this module stays
    /// provider-agnostic (see docs/20260830_plan.md, AD-1 / AD-6).
    /// </summary>
    public static IServiceCollection AddDayDashStorage(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureOptions)
    {
        services.AddDbContext<DayDashDbContext>(configureOptions);

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<IDataChangeNotifier, DataChangeNotifier>();

        return services;
    }
}
