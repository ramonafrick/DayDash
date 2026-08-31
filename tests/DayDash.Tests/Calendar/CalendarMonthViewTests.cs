using Bunit;
using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.UI.Components;
using DayDash.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DayDash.Tests.Calendar;

public class CalendarMonthViewTests : CultureIsolatedTest
{
    private static DayDashTestContext NewContext(FakeCalendarService calendar)
    {
        var ctx = new DayDashTestContext();
        ctx.Services.AddSingleton<ICalendarService>(calendar);
        return ctx;
    }

    [Fact]
    public void Renders_42_day_cells()
    {
        var calendar = new FakeCalendarService();
        using var ctx = NewContext(calendar);

        var cut = ctx.RenderComponent<CalendarMonthView>();

        Assert.Equal(42, cut.FindAll(".day-cell").Count);
    }

    [Fact]
    public void Clicking_a_day_raises_OnDaySelected_with_that_date()
    {
        var calendar = new FakeCalendarService();
        using var ctx = NewContext(calendar);
        DateOnly? selected = null;

        var cut = ctx.RenderComponent<CalendarMonthView>(p => p
            .Add(c => c.OnDaySelected, d => selected = d));

        cut.FindAll(".day-cell")[10].Click();

        Assert.NotNull(selected);
    }

    [Fact]
    public void Navigating_months_re_queries_the_visible_grid_range()
    {
        var calendar = new FakeCalendarService();
        using var ctx = NewContext(calendar);

        var cut = ctx.RenderComponent<CalendarMonthView>(); // fixture "today" = 2026-03-10
        Assert.Equal(1, calendar.RangeQueries);
        Assert.True(calendar.LastRangeQuery.From <= new DateOnly(2026, 3, 1));
        Assert.True(calendar.LastRangeQuery.To >= new DateOnly(2026, 3, 31));

        cut.FindAll("button.btn")[0].Click(); // Previous -> February
        Assert.True(calendar.LastRangeQuery.From <= new DateOnly(2026, 2, 1));

        cut.FindAll("button.btn")[1].Click(); // Next
        cut.FindAll("button.btn")[1].Click(); // Next -> April
        Assert.True(calendar.LastRangeQuery.To >= new DateOnly(2026, 4, 30));
        Assert.Equal(4, calendar.RangeQueries);
    }
}
