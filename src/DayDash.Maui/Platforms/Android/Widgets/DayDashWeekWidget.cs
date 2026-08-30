using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace DayDash.Maui;

[BroadcastReceiver(Label = "DayDash Week Widget", Exported = false)]
[IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
[MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_week_widget_info")]
public class DayDashWeekWidget : AppWidgetProvider
{
    public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
    {
        foreach (var appWidgetId in appWidgetIds ?? [])
        {
            UpdateAppWidget(context!, appWidgetManager!, appWidgetId);
        }
    }

    private static void UpdateAppWidget(Context context, AppWidgetManager appWidgetManager, int appWidgetId)
    {
        var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_week);

        // TODO: read the current week's events from the data layer.
        views.SetTextViewText(Resource.Id.widget_week_events, "This week");

        appWidgetManager.UpdateAppWidget(appWidgetId, views);
    }
}
