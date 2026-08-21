using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.IO;

namespace DayDash.Modules.Storage;

public static class StorageModuleExtensions
{
    public static IServiceCollection AddDayDashStorage(this IServiceCollection services)
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "DayDash.db");

        services.AddDbContext<DayDashDbContext>(options =>
            options.UseSqlite($"Data Source={dbPath}"));

        services.AddScoped(typeof(IRepository<>), typeof(BaseRepository<>));

        return services;
    }
}