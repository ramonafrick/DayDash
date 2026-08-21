using Microsoft.Extensions.DependencyInjection;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;

namespace DayDash.Modules.Storage
{
    public static class StorageModuleExtensions
    {
        public static IServiceCollection AddDayDashStorage(this IServiceCollection services)
        {
            services.AddScoped<IExportService, IcsExportService>();
            return services;
        }
    }
}