using System.Globalization;
using Bunit;
using DayDash.Modules.Settings.Application.Contracts;
using DayDash.Modules.Settings.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;

namespace DayDash.Tests.Infrastructure;

/// <summary>
/// bUnit context wired with the services every DayDash component expects: localization,
/// the singleton <see cref="CultureStateService"/>, a deterministic <see cref="TimeProvider"/>
/// and an in-memory <see cref="IAppPreferences"/>.
/// </summary>
public class DayDashTestContext : BunitContext
{
    public FakeTimeProvider Time { get; } = new(new DateTimeOffset(2026, 3, 10, 8, 0, 0, TimeSpan.Zero));

    public FakeAppPreferences Preferences { get; } = new();

    public DayDashTestContext()
    {
        // Component tests assert against the neutral (de-CH) resources; pin the culture so the
        // outcome does not depend on the host machine's locale. CultureIsolatedTest restores it.
        var culture = new CultureInfo(SupportedCultures.Default);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;

        Services.AddLocalization();
        Services.AddLogging();
        Services.AddSingleton<CultureStateService>();
        Services.AddSingleton<TimeProvider>(Time);
        Services.AddSingleton<IAppPreferences>(Preferences);
    }

    public CultureStateService CultureState => Services.GetRequiredService<CultureStateService>();
}

public sealed class FakeAppPreferences : IAppPreferences
{
    private readonly Dictionary<string, string> _store = new();

    public Task<string?> GetAsync(string key, string? defaultValue = null)
        => Task.FromResult(_store.TryGetValue(key, out var value) ? value : defaultValue);

    public Task SetAsync(string key, string value)
    {
        _store[key] = value;
        return Task.CompletedTask;
    }
}
