using System;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Domain;

namespace DayDash.Maui.Services;

/// <summary>
/// Android implementation of <see cref="IReminderService"/>.
/// Local notification scheduling (Android Notification Channels, daily 15:30 study
/// reminder - Requirements.md §5.4) is not wired up yet; the methods are safe no-ops.
/// </summary>
public class MauiReminderService : IReminderService
{
	public Task ScheduleDailyStudyReminderAsync(TimeOnly time, CancellationToken ct = default)
	{
		// TODO: schedule a daily repeating local notification at the given time.
		return Task.CompletedTask;
	}

	public Task ScheduleEventReminderAsync(CalendarEvent calendarEvent, int daysBefore, CancellationToken ct = default)
	{
		// TODO: schedule a one-off local notification `daysBefore` the event date.
		return Task.CompletedTask;
	}

	public Task CancelReminderAsync(Guid eventId, CancellationToken ct = default)
	{
		// TODO: cancel a previously scheduled notification.
		return Task.CompletedTask;
	}

	public Task<ReminderConfig> GetConfigAsync(CancellationToken ct = default)
		=> Task.FromResult(new ReminderConfig());

	public Task SaveConfigAsync(ReminderConfig config, CancellationToken ct = default)
		=> Task.CompletedTask;
}
