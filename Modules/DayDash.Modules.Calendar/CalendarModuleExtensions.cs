public static IServiceCollection AddDayDashCalendar(this IServiceCollection services)
{
    services.AddScoped<ICalendarService, CalendarService>();
    services.AddScoped<ICalendarRepository, CalendarRepository>();
    return services;
}