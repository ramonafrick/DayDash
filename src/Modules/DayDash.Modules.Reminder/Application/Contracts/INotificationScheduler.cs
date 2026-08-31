using DayDash.Modules.Reminder.Application.Models;

namespace DayDash.Modules.Reminder.Application.Contracts;

/// <summary>
/// Platform seam for delivering local notifications. The MAUI host implements it over
/// Plugin.LocalNotification; the browser preview and tests get a no-op.
/// </summary>
public interface INotificationScheduler
{
    /// <summary>False on hosts that cannot deliver local notifications (the browser preview).</summary>
    bool IsSupported { get; }

    Task ScheduleAsync(NotificationRequest request, CancellationToken ct = default);

    Task CancelAsync(int notificationId, CancellationToken ct = default);

    Task CancelAllAsync(CancellationToken ct = default);

    /// <summary>Requests the OS notification permission (Android 13+). Returns true when granted.</summary>
    Task<bool> RequestPermissionAsync(CancellationToken ct = default);
}
