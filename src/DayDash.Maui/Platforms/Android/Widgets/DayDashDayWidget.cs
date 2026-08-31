using Android.App;
using Android.Appwidget;
using Android.Content;
using Android.Widget;
using DayDash.Maui.Platforms.Android;
using DayDash.Modules.Widget.Application.Services;
using DayDash.Modules.Widget.Resources;

namespace DayDash.Maui;

[BroadcastReceiver(Label = "@string/widget_day_label", Exported = false)]
[IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
[MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_day_widget_info")]
public class DayDashDayWidget : AppWidgetProvider
{
    public override void OnUpdate(Context? context, AppWidgetManager? appWidgetManager, int[]? appWidgetIds)
    {
        if (context is null || appWidgetManager is null)
        {
            return;
        }

        WidgetHost.ApplyCulture();
        var snapshot = WidgetHost.Read(context, s => s.GetDayAsync());

        foreach (var appWidgetId in appWidgetIds ?? [])
        {
            var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_day);
            views.SetOnClickPendingIntent(Resource.Id.widget_root, WidgetTapIntent.OpenApp(context));
            views.SetTextViewText(Resource.Id.widget_day_title, WidgetResources.DayWidgetTitle);
            views.SetTextViewText(Resource.Id.widget_day_events, WidgetTextFormatter.DayEvents(snapshot));
            views.SetTextViewText(Resource.Id.widget_day_study_plan, WidgetTextFormatter.DayStudy(snapshot));
            appWidgetManager.UpdateAppWidget(appWidgetId, views);
        }
    }
}
