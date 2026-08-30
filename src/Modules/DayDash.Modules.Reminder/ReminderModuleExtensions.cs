using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Reminder;

public static class ReminderModuleExtensions
{
    /// <summary>
    /// Registers the Reminder module. The concrete <c>IReminderService</c> implementation is
    /// platform-specific (e.g. <c>MauiReminderService</c> on Android) and is registered by the host.
    /// </summary>
    public static IServiceCollection AddDayDashReminder(this IServiceCollection services)
    {
        return services;
    }
}
