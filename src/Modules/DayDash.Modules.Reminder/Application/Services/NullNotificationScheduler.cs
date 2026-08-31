using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Application.Models;

namespace DayDash.Modules.Reminder.Application.Services;

/// <summary>
/// No-op scheduler for hosts without local notifications (the browser preview, tests).
/// The MAUI host registers a real <see cref="INotificationScheduler"/> before the module,
/// so this only ever fills the gap.
/// </summary>
public sealed class NullNotificationScheduler : INotificationScheduler
{
    public Task ScheduleAsync(NotificationRequest request, CancellationToken ct = default) => Task.CompletedTask;

    public Task CancelAsync(int notificationId, CancellationToken ct = default) => Task.CompletedTask;

    public Task CancelAllAsync(CancellationToken ct = default) => Task.CompletedTask;

    public Task<bool> RequestPermissionAsync(CancellationToken ct = default) => Task.FromResult(false);
}
