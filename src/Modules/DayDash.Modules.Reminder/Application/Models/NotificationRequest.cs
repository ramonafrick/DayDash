namespace DayDash.Modules.Reminder.Application.Models;

/// <summary>
/// One local notification for the platform scheduler to deliver. Re-scheduling with the
/// same <see cref="Id"/> replaces the previous one.
/// </summary>
/// <param name="Id">Stable id from <see cref="Services.NotificationIds"/>.</param>
/// <param name="Title">Notification title.</param>
/// <param name="Body">Notification body.</param>
/// <param name="DeliverAt">
/// Local wall-clock time to fire (never in the past by the time it reaches the scheduler).
/// One-off - the schedule is re-derived on every app start, reboot and data change, so the
/// text is always current (FR-R2).
/// </param>
public sealed record NotificationRequest(int Id, string Title, string Body, DateTime DeliverAt);
