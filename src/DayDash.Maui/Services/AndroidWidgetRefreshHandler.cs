using Android.Appwidget;
using Android.Content;
using DayDash.Modules.Storage.Application.Contracts;

namespace DayDash.Maui.Services;

/// <summary>
/// After any persisted change, ask Android to refresh every placed DayDash widget so the
/// home screen reflects the new data (FR-W4). Interface-only coupled to the modules.
/// </summary>
public sealed class AndroidWidgetRefreshHandler : IDataChangeHandler
{
    private static readonly Type[] ProviderTypes =
    [
        typeof(DayDashDayWidget),
        typeof(DayDashWeekWidget),
        typeof(DayDashMonthWidget),
    ];

    public Task HandleAsync(DataChange change, CancellationToken ct = default)
    {
        // Only event / exam changes alter what the widgets show.
        if (change.Kind is DataChangeKind.SubjectConfigChanged
            or DataChangeKind.ReminderConfigChanged
            or DataChangeKind.EventTypeChanged)
        {
            return Task.CompletedTask;
        }

        var context = global::Android.App.Application.Context;
        var manager = AppWidgetManager.GetInstance(context);
        if (manager is null)
        {
            return Task.CompletedTask;
        }

        foreach (var providerType in ProviderTypes)
        {
            var component = new ComponentName(context, Java.Lang.Class.FromType(providerType));
            var ids = manager.GetAppWidgetIds(component);
            if (ids is not { Length: > 0 })
            {
                continue;
            }

            var intent = new Intent(AppWidgetManager.ActionAppwidgetUpdate);
            intent.SetComponent(component);
            intent.PutExtra(AppWidgetManager.ExtraAppwidgetIds, ids);
            context.SendBroadcast(intent);
        }

        return Task.CompletedTask;
    }
}
