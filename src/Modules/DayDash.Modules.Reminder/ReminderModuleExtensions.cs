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
    /// Registers the Reminder module. Falls back to a no-op <see cref="IReminderService"/>;
    /// the MAUI host registers the real Android implementation before calling this.
    /// </summary>
    public static IServiceCollection AddDayDashReminder(this IServiceCollection services)
    {
        services.AddSingleton<IModelConfiguration, ReminderModelConfiguration>();
        services.AddScoped<IDataSeeder, ReminderConfigSeeder>();

        services.AddScoped<IReminderConfigRepository, ReminderConfigRepository>();

        services.AddLocalization();
        services.TryAddScoped<IReminderService, NullReminderService>();
        return services;
    }
}
