using Android.App;
using Android.Content;
using DayDash.Modules.Reminder.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Maui.Platforms.Android;

/// <summary>
/// Re-schedules all reminders after a device reboot (alarms do not survive a restart).
/// Best effort - the next app launch reschedules anyway.
/// </summary>
[BroadcastReceiver(Enabled = true, Exported = true)]
[IntentFilter([Intent.ActionBootCompleted, Intent.ActionMyPackageReplaced])]
public sealed class BootReceiver : BroadcastReceiver
{
    public override async void OnReceive(Context? context, Intent? intent)
    {
        if (intent?.Action is not (Intent.ActionBootCompleted or Intent.ActionMyPackageReplaced))
        {
            return;
        }

        var services = IPlatformApplication.Current?.Services;
        if (services is null)
        {
            return;
        }

        var pending = GoAsync();
        try
        {
            using var scope = services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<IReminderService>().RescheduleAllAsync();
        }
        catch
        {
            // ignored - rescheduled again on next launch
        }
        finally
        {
            pending.Finish();
        }
    }
}
