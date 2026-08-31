using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Application.Services;
using DayDash.Modules.Reminder.Infrastructure;
using DayDash.Modules.Reminder.Infrastructure.Persistence;
using DayDash.Modules.Reminder.Infrastructure.Seeding;
using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DayDash.Modules.Reminder;

public static class ReminderModuleExtensions
{
    /// <summary>
    /// Registers the Reminder module. Delivery goes through <see cref="INotificationScheduler"/>;
    /// the MAUI host registers the real Android implementation before calling this, everything
    /// else falls back to a no-op.
    /// </summary>
    public static IServiceCollection AddDayDashReminder(this IServiceCollection services)
    {
        services.AddSingleton<IModelConfiguration, ReminderModelConfiguration>();
        services.AddScoped<IDataSeeder, ReminderConfigSeeder>();
        services.AddScoped<IReminderConfigRepository, ReminderConfigRepository>();

        services.AddLocalization();
        services.AddScoped<ReminderTextBuilder>();
        services.AddScoped<IReminderService, ReminderService>();
        services.AddScoped<IDataChangeHandler, ReminderRescheduleHandler>();
        services.TryAddSingleton<INotificationScheduler, NullNotificationScheduler>();

        return services;
    }
}
