using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using DayDash.Maui.Platforms.Android;
using DayDash.Modules.Widget.Application.Services;
using DayDash.Modules.Widget.Resources;

namespace DayDash.Maui;

[BroadcastReceiver(Label = "DayDash Week Widget", Exported = false)]
[IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
[MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_week_widget_info")]
public class DayDashWeekWidget : AppWidgetProvider
{
    public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
    {
        if (context is null || appWidgetManager is null)
        {
            return;
        }

        WidgetHost.ApplyCulture();
        var snapshot = WidgetHost.Read(context, s => s.GetWeekAsync());

        foreach (var appWidgetId in appWidgetIds ?? [])
        {
            var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_week);
            views.SetOnClickPendingIntent(Resource.Id.widget_root, WidgetTapIntent.OpenApp(context));
            views.SetTextViewText(Resource.Id.widget_week_title, WidgetResources.WeekWidgetTitle);
            views.SetTextViewText(Resource.Id.widget_week_events, WidgetTextFormatter.WeekEvents(snapshot));
            appWidgetManager.UpdateAppWidget(appWidgetId, views);
        }
    }
}
