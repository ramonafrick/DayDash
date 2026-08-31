using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Domain;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Calendar;

public class CalendarGridBuilderTests
{
    private static readonly IReadOnlyList<CalendarEvent> NoEvents = [];
    private static readonly IReadOnlyList<EventTypeConfig> NoTypes = [];

    private static IReadOnlyList<CalendarDayCellFlat> Flatten(int year, int month, DateOnly today,
        IReadOnlyList<CalendarEvent>? events = null, IReadOnlyList<EventTypeConfig>? types = null)
        => CalendarGridBuilder
            .Build(year, month, DayOfWeek.Monday, today, events ?? NoEvents, types ?? NoTypes)
            .SelectMany(w => w.Days)
            .Select(d => new CalendarDayCellFlat(d.Date, d.IsCurrentMonth, d.IsToday, d.EventCount, d.DotColors))
            .ToList();

    private sealed record CalendarDayCellFlat(DateOnly Date, bool IsCurrentMonth, bool IsToday, int EventCount, IReadOnlyList<string> DotColors);

    [Fact]
    public void Always_produces_six_weeks_of_seven_days()
    {
        var grid = CalendarGridBuilder.Build(2026, 3, DayOfWeek.Monday, new DateOnly(2026, 3, 10), NoEvents, NoTypes);

        Assert.Equal(6, grid.Count);
        Assert.All(grid, w => Assert.Equal(7, w.Days.Count));
    }

    [Fact]
    public void March_2026_starts_on_the_correct_leading_days()
    {
        // 1 March 2026 is a Sunday; with a Monday-start week the first row is 23 Feb .. 1 Mar.
        var cells = Flatten(2026, 3, new DateOnly(2026, 3, 10));

        Assert.Equal(new DateOnly(2026, 2, 23), cells[0].Date);
        Assert.False(cells[0].IsCurrentMonth);
        Assert.Equal(new DateOnly(2026, 3, 1), cells[6].Date);
        Assert.True(cells[6].IsCurrentMonth);
    }

    [Fact]
    public void February_2026_does_not_get_a_phantom_seventh_row()
    {
        var grid = CalendarGridBuilder.Build(2026, 2, DayOfWeek.Monday, new DateOnly(2026, 2, 1), NoEvents, NoTypes);
        Assert.Equal(6, grid.Count);
    }

    [Fact]
    public void Leap_day_2028_is_a_current_month_cell()
    {
        var cells = Flatten(2028, 2, new DateOnly(2028, 2, 1));
        var leap = cells.Single(c => c.Date == new DateOnly(2028, 2, 29));
        Assert.True(leap.IsCurrentMonth);
    }

    [Fact]
    public void A_month_starting_exactly_on_monday_has_no_leading_days()
    {
        // 1 June 2026 is a Monday.
        var cells = Flatten(2026, 6, new DateOnly(2026, 6, 1));
        Assert.Equal(new DateOnly(2026, 6, 1), cells[0].Date);
        Assert.True(cells[0].IsCurrentMonth);
    }

    [Fact]
    public void IsToday_is_set_on_exactly_one_cell_only_when_today_is_in_the_rendered_month()
    {
        var inMonth = Flatten(2026, 3, new DateOnly(2026, 3, 10));
        Assert.Single(inMonth, c => c.IsToday);
        Assert.True(inMonth.Single(c => c.IsToday).IsCurrentMonth);

        var otherMonth = Flatten(2026, 3, new DateOnly(2026, 7, 10));
        Assert.DoesNotContain(otherMonth, c => c.IsToday);
    }

    [Fact]
    public void Sunday_start_week_shifts_the_leading_days()
    {
        var grid = CalendarGridBuilder.Build(2026, 3, DayOfWeek.Sunday, new DateOnly(2026, 3, 10), NoEvents, NoTypes);
        Assert.Equal(new DateOnly(2026, 3, 1), grid[0].Days[0].Date); // 1 Mar is a Sunday
    }

    [Fact]
    public void Dots_are_deduplicated_per_type_and_use_the_navigation_colour()
    {
        var math = new EventTypeConfig { Id = Guid.NewGuid(), Key = "exam", Name = "Math", Color = "#FF0000" };
        var sport = new EventTypeConfig { Id = Guid.NewGuid(), Key = "", Name = "Sport", Color = "#00FF00" };
        var day = new DateOnly(2026, 3, 10);
        var events = new List<CalendarEvent>
        {
            TestData.AnEvent(date: day), TestData.AnEvent(date: day), TestData.AnEvent(date: day),
            TestData.AnEvent(date: day), TestData.AnEvent(date: day),
        };
        events[0].EventType = math;
        events[1].EventType = math;
        events[2].EventType = math;
        events[3].EventType = sport;
        events[4].EventType = sport;

        var cells = Flatten(2026, 3, day, events, [math, sport]);
        var cell = cells.Single(c => c.Date == day);

        Assert.Equal(5, cell.EventCount);
        Assert.Equal(2, cell.DotColors.Count);
        Assert.Contains("#FF0000", cell.DotColors);
        Assert.Contains("#00FF00", cell.DotColors);
    }

    [Fact]
    public void An_event_with_a_deleted_type_contributes_no_dot_but_still_counts()
    {
        var day = new DateOnly(2026, 3, 10);
        var e = TestData.AnEvent(date: day); // EventTypeId null, EventType null
        var cells = Flatten(2026, 3, day, [e], NoTypes);
        var cell = cells.Single(c => c.Date == day);

        Assert.Equal(1, cell.EventCount);
        Assert.Empty(cell.DotColors);
    }
}
