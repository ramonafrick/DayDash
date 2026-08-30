using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;

namespace DayDash.Maui;

[BroadcastReceiver(Label = "DayDash Month Widget", Exported = false)]
[IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
[MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_month_widget_info")]
public class DayDashMonthWidget : AppWidgetProvider
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
        var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_month);

        // TODO: render a mini month calendar with markers for days that have events.
        views.SetTextViewText(Resource.Id.widget_month_overview, "This month");

        appWidgetManager.UpdateAppWidget(appWidgetId, views);
    }
}
