using DayDash.Modules.Settings.Application.Contracts;
using DayDash.Modules.Settings.Application.Services;
using Microsoft.AspNetCore.Components;

namespace DayDash.Modules.Settings.UI.Components;

public partial class LanguageSelectorComponent
{
    /// <summary>The preference key under which the chosen culture is stored (shared with both hosts' startup code).</summary>
    public const string CulturePreferenceKey = "BlazorCulture";

    [Inject] private IAppPreferences Preferences { get; set; } = null!;

    private string _selected = SupportedCultures.Default;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _selected = SupportedCultures.Normalize(CultureState.CurrentCultureName);
    }

    private async Task OnLanguageChangedAsync(ChangeEventArgs e)
    {
        var value = SupportedCultures.Normalize(e.Value?.ToString());
        if (value == _selected)
        {
            return;
        }

        _selected = value;
        await Preferences.SetAsync(CulturePreferenceKey, value);
        CultureState.ChangeCulture(value);
    }
}
