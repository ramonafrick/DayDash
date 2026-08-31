using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Settings.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class CalendarWeekView
{
    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;
    [Inject] private ICalendarService Calendar { get; set; } = default!;
    [Inject] private TimeProvider Time { get; set; } = default!;

    [Parameter] public EventCallback<CalendarEvent> OnEventSelected { get; set; }
    [Parameter] public int RefreshToken { get; set; }

    private DateOnly _weekStart;
    private readonly List<DayColumn> _days = [];
    private int _lastRefreshToken;

    private DateOnly Today => DateOnly.FromDateTime(Time.GetLocalNow().Date);

    protected override async Task OnInitializedAsync()
    {
        _weekStart = StartOfWeek(Today);
        await ReloadAsync();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (RefreshToken != _lastRefreshToken)
        {
            _lastRefreshToken = RefreshToken;
            await ReloadAsync();
        }
    }

    private static DateOnly StartOfWeek(DateOnly date)
    {
        var diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;
        return date.AddDays(-diff);
    }

    private async Task ReloadAsync()
    {
        var events = await Calendar.GetEventsForWeekAsync(_weekStart);
        _days.Clear();
        for (var i = 0; i < 7; i++)
        {
            var date = _weekStart.AddDays(i);
            _days.Add(new DayColumn(date, events.Where(e => e.Date == date).ToList()));
        }
    }

    private async Task PreviousWeekAsync()
    {
        _weekStart = _weekStart.AddDays(-7);
        await ReloadAsync();
    }

    private async Task NextWeekAsync()
    {
        _weekStart = _weekStart.AddDays(7);
        await ReloadAsync();
    }

    private string RangeLabel
    {
        get
        {
            var c = CultureState.CurrentCulture;
            var from = _weekStart.ToDateTime(TimeOnly.MinValue).ToString("d", c);
            var to = _weekStart.AddDays(6).ToDateTime(TimeOnly.MinValue).ToString("d", c);
            return $"{from} – {to}";
        }
    }

    private string DayLabel(DateOnly date) =>
        date.ToDateTime(TimeOnly.MinValue).ToString("ddd d", CultureState.CurrentCulture);

    private sealed record DayColumn(DateOnly Date, IReadOnlyList<CalendarEvent> Events);
}
