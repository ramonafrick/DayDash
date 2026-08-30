using System.Globalization;
using DayDash.Modules.Settings.Application.Services;
using Microsoft.AspNetCore.Components;

namespace DayDash.Modules.Settings.UI;

/// <summary>
/// Base for any component that renders localized text. Subscribes to
/// <see cref="CultureStateService.CultureChanged"/> and re-renders on change, so the whole
/// UI updates live when the language is switched (Requirements.md FR-L3).
/// </summary>
public abstract class CultureAwareComponentBase : ComponentBase, IDisposable
{
    [Inject] protected CultureStateService CultureState { get; set; } = null!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        CultureState.CultureChanged += OnCultureChanged;
    }

    private void OnCultureChanged(object? sender, CultureInfo culture) => InvokeAsync(StateHasChanged);

    public virtual void Dispose() => CultureState.CultureChanged -= OnCultureChanged;
}
