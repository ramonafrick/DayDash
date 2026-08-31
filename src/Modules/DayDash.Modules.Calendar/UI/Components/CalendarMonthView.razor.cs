using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Application.Models;
using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Settings.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class CalendarMonthView
{
    private const DayOfWeek FirstDayOfWeek = DayOfWeek.Monday;

    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;
    [Inject] private ICalendarService Calendar { get; set; } = default!;
    [Inject] private TimeProvider Time { get; set; } = default!;

    /// <summary>Raised when the child wants the shell to show a day's events.</summary>
    [Parameter] public EventCallback<DateOnly> OnDaySelected { get; set; }

    /// <summary>Bumped by the parent after a write so the grid reloads.</summary>
    [Parameter] public int RefreshToken { get; set; }

    private DateOnly _month;
    private IReadOnlyList<CalendarWeek> _weeks = [];
    private int _lastRefreshToken;

    private DateOnly Today => DateOnly.FromDateTime(Time.GetLocalNow().Date);

    protected override async Task OnInitializedAsync()
    {
        _month = new DateOnly(Today.Year, Today.Month, 1);
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

    private async Task ReloadAsync()
    {
        var events = await Calendar.GetEventsForMonthAsync(_month.Year, _month.Month);
        var types = await Calendar.GetEventTypesAsync();
        _weeks = CalendarGridBuilder.Build(_month.Year, _month.Month, FirstDayOfWeek, Today, events, types);
    }

    private async Task PreviousMonthAsync()
    {
        _month = _month.AddMonths(-1);
        await ReloadAsync();
    }

    private async Task NextMonthAsync()
    {
        _month = _month.AddMonths(1);
        await ReloadAsync();
    }

    private Task SelectDayAsync(DateOnly date) => OnDaySelected.InvokeAsync(date);

    private string MonthLabel => _month.ToDateTime(TimeOnly.MinValue).ToString("MMMM yyyy", CultureState.CurrentCulture);

    private IReadOnlyList<string> WeekdayHeaders =>
    [
        Loc["Weekday_Mon"], Loc["Weekday_Tue"], Loc["Weekday_Wed"], Loc["Weekday_Thu"],
        Loc["Weekday_Fri"], Loc["Weekday_Sat"], Loc["Weekday_Sun"],
    ];
}
