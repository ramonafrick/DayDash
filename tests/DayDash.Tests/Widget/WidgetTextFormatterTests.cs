using System.Globalization;
using DayDash.Modules.Widget.Application.Models;
using DayDash.Modules.Widget.Application.Services;
using DayDash.Modules.Widget.Resources;
using Xunit;

namespace DayDash.Tests.Widget;

public class WidgetTextFormatterTests : IDisposable
{
    public void Dispose() => WidgetResources.Culture = null;

    private static void UseCulture(string name) => WidgetResources.Culture = new CultureInfo(name);

    private static WidgetDaySnapshot Day(
        IReadOnlyList<WidgetEventItem>? events = null,
        WidgetEventItem? next = null,
        IReadOnlyList<WidgetStudyItem>? study = null)
        => new(new DateOnly(2026, 3, 10), events ?? [], next, study ?? [], (study ?? []).Sum(s => s.Minutes));

    [Fact]
    public void Null_snapshot_renders_the_no_data_string()
    {
        UseCulture("de-CH");
        Assert.Equal("Keine Daten", WidgetTextFormatter.DayEvents(null));
        Assert.Equal("Keine Daten", WidgetTextFormatter.WeekEvents(null));
        Assert.Equal("Keine Daten", WidgetTextFormatter.MonthGrid(null));
    }

    [Fact]
    public void Day_events_empty_state_is_localized()
    {
        UseCulture("de-CH");
        Assert.Equal("Heute keine Termine", WidgetTextFormatter.DayEvents(Day()));

        UseCulture("en");
        Assert.Equal("No events today", WidgetTextFormatter.DayEvents(Day()));
    }

    [Fact]
    public void Day_events_falls_back_to_the_next_event_when_today_is_empty()
    {
        UseCulture("de-CH");
        var next = new WidgetEventItem("Zahnarzt", new DateOnly(2026, 3, 12), new TimeOnly(9, 0), false);

        var text = WidgetTextFormatter.DayEvents(Day(next: next));

        Assert.Contains("Nächster Termin", text);
        Assert.Contains("Zahnarzt", text);
    }

    [Fact]
    public void Day_study_shows_the_total_and_lines_or_the_empty_state()
    {
        UseCulture("de-CH");
        Assert.Equal("Heute nichts zu lernen", WidgetTextFormatter.DayStudy(Day()));

        var text = WidgetTextFormatter.DayStudy(Day(study: [new WidgetStudyItem("Mathematik", 30)]));
        Assert.Contains("Lernplan", text);
        Assert.Contains("30 Min", text);
    }

    [Fact]
    public void Week_events_empty_state_is_localized()
    {
        UseCulture("en");
        Assert.Equal("No events this week", WidgetTextFormatter.WeekEvents(new WidgetWeekSnapshot(new DateOnly(2026, 3, 9), [])));
    }

    [Fact]
    public void Month_grid_has_a_header_and_six_week_rows()
    {
        UseCulture("de-CH");
        var days = Enumerable.Range(0, 42)
            .Select(i =>
            {
                var date = new DateOnly(2026, 2, 23).AddDays(i); // grid start for March 2026 (Mon)
                return new WidgetMonthDay(date, date.Month == 3, date == new DateOnly(2026, 3, 10), date.Day is 12 && date.Month == 3);
            })
            .ToList();
        var snapshot = new WidgetMonthSnapshot(2026, 3, days, [12]);

        var grid = WidgetTextFormatter.MonthGrid(snapshot);

        Assert.Equal(7, grid.Split('\n').Length);
        Assert.StartsWith("Mo Di Mi", grid);
    }
}
