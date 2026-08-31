using DayDash.Modules.Calendar.Infrastructure.Seeding;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Reminder.Infrastructure.Seeding;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.StudyPlanner.Infrastructure.Seeding;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;

namespace DayDash.Tests.Infrastructure;

/// <summary>
/// Builds the real seeder chain (with real resx-backed localizers) + a
/// <see cref="DatabaseInitializer"/> against a supplied context, for seeder / initializer tests.
/// </summary>
public static class SeedingHost
{
    private static readonly IServiceProvider Localization =
        new ServiceCollection().AddLogging().AddLocalization().BuildServiceProvider();

    public static IReadOnlyList<IDataSeeder> Seeders() =>
    [
        new EventTypeSeeder(Localization.GetRequiredService<IStringLocalizer<CalendarResources>>()),
        new SubjectConfigSeeder(Localization.GetRequiredService<IStringLocalizer<StudyPlannerResources>>()),
        new ReminderConfigSeeder(),
    ];

    public static DatabaseInitializer Initializer(DayDashDbContext context) =>
        new(context, Seeders(), NullLogger<DatabaseInitializer>.Instance);
}
