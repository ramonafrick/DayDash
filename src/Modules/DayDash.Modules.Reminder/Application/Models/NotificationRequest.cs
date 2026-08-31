namespace DayDash.Modules.Reminder.Application.Models;

/// <summary>
/// One local notification for the platform scheduler to deliver. Re-scheduling with the
/// same <see cref="Id"/> replaces the previous one.
/// </summary>
/// <param name="Id">Stable id from <see cref="Services.NotificationIds"/>.</param>
/// <param name="Title">Notification title.</param>
/// <param name="Body">Notification body.</param>
/// <param name="DeliverAt">Local time to fire (never in the past by the time it reaches the scheduler).</param>
/// <param name="RepeatDaily">When true, repeats every day at the same wall-clock time.</param>
public sealed record NotificationRequest(int Id, string Title, string Body, DateTimeOffset DeliverAt, bool RepeatDaily = false);
