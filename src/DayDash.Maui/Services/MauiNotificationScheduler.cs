using DayDash.Modules.Reminder.Application.Contracts;
using Plugin.LocalNotification;
using AppNotificationRequest = DayDash.Modules.Reminder.Application.Models.NotificationRequest;
using CoreModels = Plugin.LocalNotification.Core.Models;

namespace DayDash.Maui.Services;

/// <summary>
/// Delivers reminders through Plugin.LocalNotification. Thin platform adapter - all
/// scheduling logic lives in the module's <see cref="IReminderService"/>.
/// </summary>
public sealed class MauiNotificationScheduler : INotificationScheduler
{
    internal const string ChannelId = "daydash_reminders";

    public async Task ScheduleAsync(AppNotificationRequest request, CancellationToken ct = default)
    {
        var plugin = new CoreModels.NotificationRequest
        {
            NotificationId = request.Id,
            Title = request.Title,
            Description = request.Body,
            Android = new CoreModels.AndroidOption.AndroidOptions
            {
                ChannelId = ChannelId,
                Priority = CoreModels.AndroidOption.AndroidPriority.High,
            },
            Schedule = new CoreModels.NotificationRequestSchedule
            {
                NotifyTime = request.DeliverAt.LocalDateTime,
                RepeatType = request.RepeatDaily
                    ? CoreModels.NotificationRepeat.Daily
                    : CoreModels.NotificationRepeat.No,
            },
        };

        await LocalNotificationCenter.Current.Show(plugin);
    }

    public Task CancelAsync(int notificationId, CancellationToken ct = default)
    {
        LocalNotificationCenter.Current.Cancel(notificationId);
        return Task.CompletedTask;
    }

    public Task CancelAllAsync(CancellationToken ct = default)
    {
        LocalNotificationCenter.Current.CancelAll();
        return Task.CompletedTask;
    }

    public async Task<bool> RequestPermissionAsync(CancellationToken ct = default)
        => await LocalNotificationCenter.Current.RequestNotificationPermission(new CoreModels.NotificationPermission());
}
