using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using Microsoft.JSInterop;
using DayDash.Modules.Settings.Resources;

namespace DayDash.Modules.Settings.UI.Components;

public partial class ThemeToggle
{
    [Inject] private IJSRuntime JsRuntime { get; set; } = null!;
    [Inject] private IStringLocalizer<SettingsResources> Loc { get; set; } = null!;

    private const string LightTheme = "light";
    private const string DarkTheme = "dark";

    private string _currentTheme = LightTheme;

    private string ToggleTitle =>
        _currentTheme == LightTheme ? Loc["ThemeDark"] : Loc["ThemeLight"];

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
        {
            return;
        }

        try
        {
            _currentTheme = await JsRuntime.InvokeAsync<string?>("localStorage.getItem", "theme") ?? LightTheme;
            await ApplyThemeAsync(_currentTheme);
            StateHasChanged();
        }
        catch (JSException)
        {
            // Pre-render / storage unavailable - keep the light default.
        }
    }

    private async Task ToggleThemeAsync()
    {
        _currentTheme = _currentTheme == LightTheme ? DarkTheme : LightTheme;
        await ApplyThemeAsync(_currentTheme);
        try
        {
            await JsRuntime.InvokeVoidAsync("localStorage.setItem", "theme", _currentTheme);
        }
        catch (JSException)
        {
            // Non-fatal: the theme still applies for this session.
        }
    }

    private async Task ApplyThemeAsync(string theme)
    {
        try
        {
            await JsRuntime.InvokeVoidAsync("eval",
                $"document.documentElement.setAttribute('data-theme', '{theme}')");
        }
        catch (JSException)
        {
            // Ignore - nothing to toggle when JS is unavailable.
        }
    }
}
