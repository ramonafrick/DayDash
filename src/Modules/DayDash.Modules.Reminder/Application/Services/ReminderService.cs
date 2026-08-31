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

        await ScheduleDailyStudyReminderAsync(config, ct);
        await ScheduleEventRemindersAsync(config, ct);
    }

    private async Task ScheduleDailyStudyReminderAsync(ReminderConfig config, CancellationToken ct)
    {
        var todaysExams = await studyPlanner.GetTodayStudyPlanAsync(ct);
        var totalMinutes = todaysExams.Sum(e => e.DailyMinutes);

        var body = textBuilder.DailyStudyBody(todaysExams, totalMinutes);
        if (body is null)
        {
            return; // FR-R2: only fire when there is something to study today
        }

        var now = timeProvider.GetLocalNow();
        var deliverAt = new DateTimeOffset(
            DateOnly.FromDateTime(now.Date).ToDateTime(config.DailyStudyReminderTime), now.Offset);
        if (deliverAt <= now)
        {
            deliverAt = deliverAt.AddDays(1);
        }

        await scheduler.ScheduleAsync(
            new NotificationRequest(NotificationIds.DailyStudyReminder, textBuilder.DailyStudyTitle, body, deliverAt, RepeatDaily: true),
            ct);
    }

    private async Task ScheduleEventRemindersAsync(ReminderConfig config, CancellationToken ct)
    {
        var now = timeProvider.GetLocalNow();
        var today = DateOnly.FromDateTime(now.Date);
        var events = await calendar.GetEventsInRangeAsync(today, today.AddDays(EventWindowDays), ct);

        foreach (var e in events)
        {
            var lead = e.ReminderDaysBefore ?? config.EventReminderDaysBefore;
            if (lead < 0)
            {
                continue;
            }

            var fireTime = e.IsAllDay ? DefaultEventTime : e.TimeFrom ?? DefaultEventTime;
            var deliverAt = new DateTimeOffset(e.Date.AddDays(-lead).ToDateTime(fireTime), now.Offset);
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
