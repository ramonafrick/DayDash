using Android.Content;
using DayDash.Migrations;
using DayDash.Modules.Settings.Application.Services;
using DayDash.Modules.Settings.UI.Components;
using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.Widget.Application.Contracts;
using DayDash.Modules.Widget.Application.Services;
using DayDash.Modules.Widget.Resources;

namespace DayDash.Maui.Platforms.Android;

/// <summary>
/// Shared plumbing for the home-screen widgets. They run in a BroadcastReceiver with no
/// service provider, so this applies the stored UI culture and opens a short-lived,
/// read-only <see cref="DayDashDbContext"/> straight from the database file. Any failure
/// yields <c>default</c> so the widget can fall back to its localized empty state.
/// </summary>
internal static class WidgetHost
{
    public static void ApplyCulture()
    {
        try
        {
            var stored = Preferences.Get(LanguageSelectorComponent.CulturePreferenceKey, SupportedCultures.Default);
            WidgetResources.Culture = SupportedCultures.Resolve(stored);
        }
        catch
        {
            WidgetResources.Culture = SupportedCultures.Resolve(SupportedCultures.Default);
        }
    }

    public static T? Read<T>(Context context, Func<IWidgetDataService, Task<T>> query)
    {
        try
        {
            var path = System.IO.Path.Combine(context.FilesDir!.AbsolutePath, DayDashDatabase.FileName);
            if (!System.IO.File.Exists(path))
            {
                return default;
            }

            using var db = DayDashDbContextFactory.CreateSqlite(path, readOnly: true);
            var service = new WidgetDataService(db, TimeProvider.System);
            return query(service).GetAwaiter().GetResult();
        }
        catch
        {
            return default;
        }
    }
}
