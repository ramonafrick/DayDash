using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Reminder.Domain;

namespace DayDash.Modules.Reminder.Application.Contracts;

public interface IReminderService
{
    Task ScheduleDailyStudyReminderAsync(TimeOnly time, CancellationToken ct = default);
    Task ScheduleEventReminderAsync(CalendarEvent calendarEvent, int daysBefore, CancellationToken ct = default);
    Task CancelReminderAsync(Guid eventId, CancellationToken ct = default);
    Task<ReminderConfig> GetConfigAsync(CancellationToken ct = default);
    Task SaveConfigAsync(ReminderConfig config, CancellationToken ct = default);
}