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

    private CultureChangeSubscription? _subscription;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _subscription = new CultureChangeSubscription(CultureState, () => InvokeAsync(StateHasChanged));
        _subscription.Start();
    }

    public virtual void Dispose() => _subscription?.Dispose();
}
