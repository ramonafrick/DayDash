using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Application.Models;
using DayDash.Modules.Reminder.Domain;
using DayDash.Modules.StudyPlanner.Application.Contracts;

namespace DayDash.Modules.Reminder.Application.Services;

/// <summary>
/// Owns the reminder config and re-derives the whole notification schedule from the current
/// data. Platform-agnostic (all delivery goes through <see cref="INotificationScheduler"/>)
/// and fully unit-testable.
/// </summary>
public sealed class ReminderService(
    IReminderConfigRepository configRepository,
    INotificationScheduler scheduler,
    ReminderTextBuilder textBuilder,
    IStudyPlannerService studyPlanner,
    ICalendarService calendar,
    TimeProvider timeProvider) : IReminderService
{
    /// <summary>How far ahead one-off event reminders are scheduled.</summary>
    private const int EventWindowDays = 60;

    /// <summary>Fallback time of day for an event with no start time.</summary>
    private static readonly TimeOnly DefaultEventTime = new(8, 0);

    public Task<ReminderConfig> GetConfigAsync(CancellationToken ct = default)
        => configRepository.GetAsync(ct);

    public async Task SaveConfigAsync(ReminderConfig config, CancellationToken ct = default)
    {
        await configRepository.SaveAsync(config, ct);
        await RescheduleAllAsync(ct);
    }

    public async Task RescheduleAllAsync(CancellationToken ct = default)
    {
        var config = await configRepository.GetAsync(ct);

        // No persistent registry of scheduled ids, so clear everything and rebuild (FR-R6).
        await scheduler.CancelAllAsync(ct);

        if (!config.IsEnabled)
        {
            return;
        }

        // Manifest permission is not enough on Android 13+; ask once (no-op elsewhere).
        await scheduler.RequestPermissionAsync(ct);

        var now = timeProvider.GetLocalNow().DateTime;
        await ScheduleDailyStudyReminderAsync(config, now, ct);
        await ScheduleEventRemindersAsync(config, now, ct);
    }

    private async Task ScheduleDailyStudyReminderAsync(ReminderConfig config, DateTime now, CancellationToken ct)
    {
        var todaysExams = await studyPlanner.GetTodayStudyPlanAsync(ct);
        var totalMinutes = todaysExams.Sum(e => e.DailyMinutes);

        var body = textBuilder.DailyStudyBody(todaysExams, totalMinutes);
        if (body is null)
        {
            return; // FR-R2: only fire when there is something to study today
        }

        // One-off for the next occurrence; re-armed on every app start / reboot / data change,
        // so it never keeps firing with stale text after the exams are done.
        var deliverAt = DateOnly.FromDateTime(now).ToDateTime(config.DailyStudyReminderTime);
        if (deliverAt <= now)
        {
            deliverAt = deliverAt.AddDays(1);
        }

        await scheduler.ScheduleAsync(
            new NotificationRequest(NotificationIds.DailyStudyReminder, textBuilder.DailyStudyTitle, body, deliverAt),
            ct);
    }

    private async Task ScheduleEventRemindersAsync(ReminderConfig config, DateTime now, CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(now);
        var events = await calendar.GetEventsInRangeAsync(today, today.AddDays(EventWindowDays), ct);

        foreach (var e in events)
        {
            var lead = e.ReminderDaysBefore ?? config.EventReminderDaysBefore;
            if (lead < 0)
            {
                continue;
            }

            var fireTime = e.IsAllDay ? DefaultEventTime : e.TimeFrom ?? DefaultEventTime;
            var deliverAt = e.Date.AddDays(-lead).ToDateTime(fireTime);
            if (deliverAt <= now)
            {
                continue; // never schedule in the past
            }

            await scheduler.ScheduleAsync(
                new NotificationRequest(
                    NotificationIds.ForEvent(e.Id),
                    textBuilder.EventTitle,
                    textBuilder.EventBody(e.Title, e.Date),
                    deliverAt),
                ct);
        }
    }
}
