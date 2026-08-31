using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Resources;
using Microsoft.Extensions.Localization;
using Plugin.LocalNotification;
using AppNotificationRequest = DayDash.Modules.Reminder.Application.Models.NotificationRequest;
using CoreModels = Plugin.LocalNotification.Core.Models;

namespace DayDash.Maui.Services;

/// <summary>
/// Delivers reminders through Plugin.LocalNotification. Thin platform adapter - all
/// scheduling logic lives in the module's <see cref="IReminderService"/>.
/// </summary>
public sealed class MauiNotificationScheduler(IStringLocalizer<ReminderResources> loc) : INotificationScheduler
{
    private const string ChannelId = "daydash_reminders";

    private bool _channelReady;

    public bool IsSupported => true;

    public async Task ScheduleAsync(AppNotificationRequest request, CancellationToken ct = default)
    {
        EnsureChannel();

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
                NotifyTime = request.DeliverAt,
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

    /// <summary>Android 8+ drops notifications posted to an unknown channel. Create it once.</summary>
    private void EnsureChannel()
    {
        if (_channelReady)
        {
            return;
        }

        LocalNotificationCenter.CreateNotificationChannels(
        [
            new CoreModels.AndroidOption.AndroidNotificationChannelRequest
            {
                Id = ChannelId,
                Name = loc["NotificationChannelName"],
                Description = loc["NotificationChannelDescription"],
                Importance = CoreModels.AndroidOption.AndroidImportance.High,
            },
        ]);
        _channelReady = true;
    }
}
