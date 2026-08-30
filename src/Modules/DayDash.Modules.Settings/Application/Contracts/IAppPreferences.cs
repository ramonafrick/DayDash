namespace DayDash.Modules.Settings.Application.Contracts;

/// <summary>
/// Host-provided key/value preference store. Backed by <c>Microsoft.Maui.Storage.Preferences</c>
/// on Android and by <c>localStorage</c> in the Blazor WebAssembly preview host.
/// It is async because the browser implementation goes through JS interop.
/// </summary>
public interface IAppPreferences
{
    Task<string?> GetAsync(string key, string? defaultValue = null);

    Task SetAsync(string key, string value);
}
