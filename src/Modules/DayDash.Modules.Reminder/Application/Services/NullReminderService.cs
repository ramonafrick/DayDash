using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Domain;

namespace DayDash.Modules.Reminder.Application.Services;

/// <summary>
/// No-op fallback used where no platform scheduler exists (the browser preview host, tests).
/// The MAUI host replaces this with a real implementation.
/// </summary>
public sealed class NullReminderService : IReminderService
{
    public Task ScheduleDailyStudyReminderAsync(TimeOnly time, CancellationToken ct = default) => Task.CompletedTask;

    public Task ScheduleEventReminderAsync(CalendarEvent calendarEvent, int daysBefore, CancellationToken ct = default) => Task.CompletedTask;

    public Task CancelReminderAsync(Guid eventId, CancellationToken ct = default) => Task.CompletedTask;

    public Task<ReminderConfig> GetConfigAsync(CancellationToken ct = default) => Task.FromResult(new ReminderConfig());

    public Task SaveConfigAsync(ReminderConfig config, CancellationToken ct = default) => Task.CompletedTask;
}
