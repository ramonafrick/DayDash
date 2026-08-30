using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Calendar;

public static class CalendarModuleExtensions
{
    public static IServiceCollection AddDayDashCalendar(this IServiceCollection services)
    {
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<ICalendarRepository, CalendarRepository>();
        services.AddScoped<IExportService, IcsExportService>();
        return services;
    }
}
