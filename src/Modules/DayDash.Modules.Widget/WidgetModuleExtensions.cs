using DayDash.Modules.Widget.Application.Contracts;
using DayDash.Modules.Widget.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Widget;

public static class WidgetModuleExtensions
{
    /// <summary>
    /// Registers the Widget module. The Android home-screen widgets themselves are implemented
    /// as platform-specific <c>AppWidgetProvider</c>s in the MAUI host and build their own
    /// read-only <c>DbContext</c>; this registration exists so the read model is available
    /// through DI as well, and so the module can be added or removed like every other module.
    /// </summary>
    public static IServiceCollection AddDayDashWidget(this IServiceCollection services)
    {
        services.AddScoped<IWidgetDataService, WidgetDataService>();
        return services;
    }
}
