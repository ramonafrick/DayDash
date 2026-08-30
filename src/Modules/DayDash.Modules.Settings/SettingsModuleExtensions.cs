using DayDash.Modules.Settings.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DayDash.Modules.Settings;

public static class SettingsModuleExtensions
{
    /// <summary>
    /// Registers the Settings module: the app-wide culture state service and localization.
    /// The host supplies the platform services (<c>IAppPreferences</c>, <c>IFileShareService</c>).
    /// </summary>
    public static IServiceCollection AddDayDashSettings(this IServiceCollection services)
    {
        services.AddSingleton<CultureStateService>();
        services.AddLocalization();
        return services;
    }
}
