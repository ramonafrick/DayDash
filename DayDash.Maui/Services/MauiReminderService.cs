using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Application.Models;
using DayDash.Modules.Reminder.Domain;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace DayDash.Maui.Services;

public class MauiReminderService : IReminderService
{
    public async Task ScheduleDailyStudyReminderAsync(TimeOnly time, CancellationToken ct = default)
    {
        var notification = new NotificationRequest
        {
            NotificationId = Guid.NewGuid().GetHashCode(),
            Title = "DayDash – Lernen",
            Description = "Zeit zum Lernen für deine Prüfung!",
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = DateTime.Today.Add(time.ToTimeSpan()),
                RepeatType = NotificationRepeat.Daily
            }
        };

        await NotificationCenter.Current.Show(notification);
    }

    public async Task ScheduleEventReminderAsync(CalendarEvent calendarEvent, int daysBefore, CancellationToken ct = default)
    {
        var notifyTime = calendarEvent.StartDate.AddDays(-daysBefore);
        var notification = new NotificationRequest
        {
            NotificationId = calendarEvent.Id.GetHashCode(),
            Title = calendarEvent.Title,
            Description = "Erinnerung: " + calendarEvent.Description,
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = notifyTime
            }
        };

        await NotificationCenter.Current.Show(notification);
    }

    public async Task CancelReminderAsync(Guid eventId, CancellationToken ct = default)
    {
        await NotificationCenter.Current.Cancel(eventId.GetHashCode());
    }

    public Task<ReminderConfig> GetConfigAsync(CancellationToken ct = default)
    {
        // Placeholder for actual implementation
        return Task.FromResult(new ReminderConfig());
    }

    public Task SaveConfigAsync(ReminderConfig config, CancellationToken ct = default)
    {
        // Placeholder for actual implementation
        return Task.CompletedTask;
    }
}