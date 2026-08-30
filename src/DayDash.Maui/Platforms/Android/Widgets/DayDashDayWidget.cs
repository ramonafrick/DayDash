using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace DayDash.Maui;

[BroadcastReceiver(Label = "DayDash Day Widget", Exported = false)]
[IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
[MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_day_widget_info")]
public class DayDashDayWidget : AppWidgetProvider
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
        var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_day);

        // TODO: read today's events and study plan from the data layer.
        views.SetTextViewText(Resource.Id.widget_day_events, "Today's Events");
        views.SetTextViewText(Resource.Id.widget_day_study_plan, "Study Plan");

        appWidgetManager.UpdateAppWidget(appWidgetId, views);
    }
}
