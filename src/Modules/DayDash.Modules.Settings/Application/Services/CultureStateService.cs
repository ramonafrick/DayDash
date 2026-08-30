using System.Globalization;
using Microsoft.Extensions.Logging;

namespace DayDash.Modules.Settings.Application.Services;

/// <summary>
/// App-wide singleton that owns the current UI culture and notifies subscribers when it
/// changes, so language switching takes effect live without an app restart
/// (Requirements.md FR-L3). Ported from MiniMate's <c>CultureStateService</c>.
/// </summary>
public sealed class CultureStateService(ILogger<CultureStateService> logger)
{
    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

    public event EventHandler<CultureInfo>? CultureChanged;

    public CultureInfo CurrentCulture => _currentCulture;

    public string CurrentCultureName => _currentCulture.Name;

    public void ChangeCulture(CultureInfo newCulture)
    {
        if (string.Equals(_currentCulture.Name, newCulture.Name, StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        logger.LogInformation("Culture changed from {Old} to {New}", _currentCulture.Name, newCulture.Name);

        _currentCulture = newCulture;

        CultureInfo.DefaultThreadCurrentCulture = newCulture;
        CultureInfo.DefaultThreadCurrentUICulture = newCulture;
        CultureInfo.CurrentCulture = newCulture;
        CultureInfo.CurrentUICulture = newCulture;

        CultureChanged?.Invoke(this, newCulture);
    }

    public void ChangeCulture(string cultureName) => ChangeCulture(SupportedCultures.Resolve(cultureName));
}
