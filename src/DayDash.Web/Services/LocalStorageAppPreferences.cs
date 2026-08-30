using DayDash.Modules.Settings.Application.Contracts;
using Microsoft.JSInterop;

namespace DayDash.Web.Services;

/// <summary>Browser-backed <see cref="IAppPreferences"/> using <c>localStorage</c>.</summary>
public sealed class LocalStorageAppPreferences(IJSRuntime js) : IAppPreferences
{
    public async Task<string?> GetAsync(string key, string? defaultValue = null)
    {
        try
        {
            return await js.InvokeAsync<string?>("localStorage.getItem", key) ?? defaultValue;
        }
        catch (JSException)
        {
            return defaultValue;
        }
    }

    public async Task SetAsync(string key, string value)
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.setItem", key, value);
        }
        catch (JSException)
        {
            // Non-fatal in the preview host.
        }
    }
}
