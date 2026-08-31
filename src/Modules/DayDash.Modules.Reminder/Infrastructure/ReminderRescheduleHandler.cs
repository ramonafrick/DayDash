using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Storage.Application.Contracts;

namespace DayDash.Modules.Reminder.Infrastructure;

/// <summary>
/// Any persisted change (an exam or event saved/deleted, a subject or event-type edited)
/// can move the study load or an event date, so re-derive the whole schedule (FR-R6).
/// </summary>
public sealed class ReminderRescheduleHandler(IReminderService reminders) : IDataChangeHandler
{
    public Task HandleAsync(DataChange change, CancellationToken ct = default)
        => change.Kind switch
        {
            // Only changes that can move the study load or an event date matter here.
            DataChangeKind.EventTypeChanged => Task.CompletedTask,
            _ => reminders.RescheduleAllAsync(ct),
        };
}
