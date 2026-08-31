using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Infrastructure;
using DayDash.Modules.Calendar.Infrastructure.Persistence;
using DayDash.Modules.Calendar.Infrastructure.Seeding;
using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Calendar;

public static class CalendarModuleExtensions
{
    public static IServiceCollection AddDayDashCalendar(this IServiceCollection services)
    {
        services.AddSingleton<IModelConfiguration, CalendarModelConfiguration>();
        services.AddScoped<IDataSeeder, EventTypeSeeder>();
        services.AddScoped<IDataChangeHandler, CalendarExamLinkHandler>();

        services.AddScoped<ICalendarRepository, CalendarRepository>();
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<IExportService, IcsExportService>();

        services.AddLocalization();
        return services;
    }
}
