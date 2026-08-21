using Android.Appwidget;
using Android.Content;
using Android.Widget;
using System;
using System.Threading.Tasks;

namespace DayDash.Maui.Platforms.Android.Widgets
{
    [BroadcastReceiver(Label = "DayDash Month Widget")]
    [IntentFilter(new[] { AppWidgetManager.ActionAppwidgetUpdate })]
    [MetaData(AppWidgetManager.MetaDataAppwidgetProvider, Resource = "@xml/daydash_month_widget_info")]
    public class DayDashMonthWidget : AppWidgetProvider
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
            var views = new RemoteViews(context.PackageName, Resource.Layout.widget_daydash_month);

            // Fetch data (e.g., mini month calendar with event dots)
            var monthOverview = "Mini Calendar with Event Dots"; // Placeholder

            views.SetTextViewText(Resource.Id.widget_month_overview, monthOverview);

            appWidgetManager.UpdateAppWidget(appWidgetId, views);
        }
    }
}