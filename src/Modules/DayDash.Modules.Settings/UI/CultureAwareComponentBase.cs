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

    private CultureChangeSubscription? _subscription;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        _subscription = new CultureChangeSubscription(CultureState, () => InvokeAsync(StateHasChanged));
        _subscription.Start();
    }

    public virtual void Dispose() => _subscription?.Dispose();
}
