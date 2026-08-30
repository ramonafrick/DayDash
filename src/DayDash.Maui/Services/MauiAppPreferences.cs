using DayDash.Modules.Settings.Application.Contracts;

namespace DayDash.Maui.Services;

/// <summary><see cref="IAppPreferences"/> over <c>Microsoft.Maui.Storage.Preferences</c>.</summary>
public sealed class MauiAppPreferences : IAppPreferences
{
	public Task<string?> GetAsync(string key, string? defaultValue = null)
		=> Task.FromResult(Preferences.Get(key, defaultValue));

	public Task SetAsync(string key, string value)
	{
		Preferences.Set(key, value);
		return Task.CompletedTask;
	}
}
