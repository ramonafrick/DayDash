using DayDash.Modules.Widget.Application.Models;

namespace DayDash.Modules.Widget.Application.Contracts;

/// <summary>
/// Read-only projections of the shared database for the Android home-screen widgets.
/// Every method is a bounded, no-tracking query - widgets never write.
/// </summary>
public interface IWidgetDataService
{
    Task<WidgetDaySnapshot> GetDayAsync(CancellationToken ct = default);

    Task<WidgetWeekSnapshot> GetWeekAsync(CancellationToken ct = default);

    Task<WidgetMonthSnapshot> GetMonthAsync(CancellationToken ct = default);
}
