using Android.Appwidget;
using Android.Content;
using Android.Widget;
using System;
using System.Threading.Tasks;

namespace DayDash.Maui.Platforms.Android.Widgets
{
    [BroadcastReceiver(Label = "DayDash Week Widget")]
    [IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_week_widget_info")]
    public class DayDashWeekWidget : AppWidgetProvider
    {
        public override void OnUpdate(Context context, AppWidgetManager appWidgetManager, int[] appWidgetIds)
        {
            foreach (var appWidgetId in appWidgetIds)
            {
                UpdateAppWidget(context, appWidgetManager, appWidgetId);
            }
        }

        private void UpdateAppWidget(Context context, AppWidgetManager appWidgetManager, int appWidgetId)
        {
            var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_week);

            // Fetch data (e.g., weekly events)
            var weeklyEvents = "Mon: Math Exam\nTue: Soccer Practice\n..."; // Placeholder

            views.SetTextViewText(Resource.Id.widget_week_events, weeklyEvents);

            appWidgetManager.UpdateAppWidget(appWidgetId, views);
        }
    }
}