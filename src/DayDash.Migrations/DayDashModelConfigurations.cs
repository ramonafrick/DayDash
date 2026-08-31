using DayDash.Modules.Calendar.Infrastructure.Persistence;
using DayDash.Modules.Reminder.Infrastructure.Persistence;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Infrastructure.Persistence;

namespace DayDash.Migrations;

/// <summary>
/// The full set of <see cref="IModelConfiguration"/> contributions, in a fixed order, used by
/// the design-time / widget construction paths where DI is not available. The DI path
/// (app hosts, tests) enumerates the same implementations from the container instead.
/// </summary>
public static class DayDashModelConfigurations
{
    public static IReadOnlyList<IModelConfiguration> All { get; } =
    [
        new CalendarModelConfiguration(),
        new StudyPlannerModelConfiguration(),
        new ReminderModelConfiguration(),
    ];
}
