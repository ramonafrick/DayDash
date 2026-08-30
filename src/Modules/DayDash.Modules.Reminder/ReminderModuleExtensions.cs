using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DayDash.Modules.Reminder;

public static class ReminderModuleExtensions
{
    /// <summary>
    /// Registers the Reminder module. Falls back to a no-op <see cref="IReminderService"/>;
    /// the MAUI host registers the real Android implementation before calling this.
    /// </summary>
    public static IServiceCollection AddDayDashReminder(this IServiceCollection services)
    {
        services.AddLocalization();
        services.TryAddScoped<IReminderService, NullReminderService>();
        return services;
    }
}
