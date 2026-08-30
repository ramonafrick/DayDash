using System.Globalization;
using DayDash.Modules.Settings.Application.Services;
using Microsoft.AspNetCore.Components;

namespace DayDash.Modules.Settings.UI;

/// <summary>
/// <see cref="LayoutComponentBase"/> variant of <see cref="CultureAwareComponentBase"/> so the
/// shared <c>MainLayout</c> re-renders its chrome (nav labels, links) on a live language switch.
/// </summary>
public abstract class CultureAwareLayoutComponentBase : LayoutComponentBase, IDisposable
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
