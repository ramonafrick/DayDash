using System.Globalization;
using DayDash.Modules.Settings.Application.Services;

namespace DayDash.Modules.Settings.UI;

/// <summary>
/// Shared wiring for <see cref="CultureAwareComponentBase"/> and
/// <see cref="CultureAwareLayoutComponentBase"/> (which live in different component
/// hierarchies and so cannot share a base class): subscribe on start, invoke a re-render
/// callback on change, unsubscribe on dispose.
/// </summary>
internal sealed class CultureChangeSubscription(CultureStateService cultureState, Func<Task> onChanged) : IDisposable
{
    public void Start() => cultureState.CultureChanged += Handle;

    private void Handle(object? sender, CultureInfo culture) => _ = onChanged();

    public void Dispose() => cultureState.CultureChanged -= Handle;
}
