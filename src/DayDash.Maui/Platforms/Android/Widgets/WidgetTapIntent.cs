using Android.App;
using Android.Content;

namespace DayDash.Maui.Platforms.Android;

/// <summary>A tap on any widget opens the app (tap-to-open only, no deep link).</summary>
internal static class WidgetTapIntent
{
    public static PendingIntent? OpenApp(Context context)
    {
        var launch = new Intent(context, typeof(MainActivity));
        launch.AddFlags(ActivityFlags.NewTask | ActivityFlags.SingleTop);

        return PendingIntent.GetActivity(
            context, 0, launch, PendingIntentFlags.UpdateCurrent | PendingIntentFlags.Immutable);
    }
}
