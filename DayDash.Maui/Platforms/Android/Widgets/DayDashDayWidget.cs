using Android.Appwidget;
using Android.Content;
using Android.Widget;
using System;
using System.Threading.Tasks;

namespace DayDash.Maui.Platforms.Android.Widgets
{
    [BroadcastReceiver(Label = "DayDash Day Widget")]
    [IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_day_widget_info")]
    public class DayDashDayWidget : AppWidgetProvider
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
            var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_day);

            // Fetch data (e.g., today's events and study plan)
            var events = "Today's Events: Math Exam, Soccer Practice"; // Placeholder
            var studyPlan = "Study Plan: 2 hours of Math"; // Placeholder

            views.SetTextViewText(Resource.Id.widget_day_events, events);
            views.SetTextViewText(Resource.Id.widget_day_study_plan, studyPlan);

            appWidgetManager.UpdateAppWidget(appWidgetId, views);
        }
    }
}