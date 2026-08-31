using System.Globalization;

namespace DayDash.Tests.Infrastructure;

/// <summary>
/// Base for tests that mutate the process-global <see cref="CultureInfo"/> statics
/// (via <c>CultureStateService</c>). Captures the culture on construction and restores it on
/// dispose so a switch in one test can't leak into another. Tests run serially
/// (xunit.runner.json) so a save/restore pair is enough.
/// </summary>
public abstract class CultureIsolatedTest : IDisposable
{
    private readonly CultureInfo _culture = CultureInfo.CurrentCulture;
    private readonly CultureInfo _uiCulture = CultureInfo.CurrentUICulture;

    public void Dispose()
    {
        CultureInfo.CurrentCulture = _culture;
        CultureInfo.CurrentUICulture = _uiCulture;
        CultureInfo.DefaultThreadCurrentCulture = _culture;
        CultureInfo.DefaultThreadCurrentUICulture = _uiCulture;
        GC.SuppressFinalize(this);
    }
}
