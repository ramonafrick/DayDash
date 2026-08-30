using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Widget;

public static class WidgetModuleExtensions
{
    /// <summary>
    /// Registers the Widget module. The Android home-screen widgets themselves are implemented
    /// as platform-specific <c>AppWidgetProvider</c>s in the MAUI host; this hook exists so the
    /// module can be added or removed like every other module.
    /// </summary>
    public static IServiceCollection AddDayDashWidget(this IServiceCollection services)
    {
        return services;
    }
}
