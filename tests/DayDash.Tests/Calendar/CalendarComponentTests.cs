using Bunit;
using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.UI.Components;
using DayDash.Modules.Settings.Application.Contracts;
using DayDash.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DayDash.Tests.Calendar;

public class CalendarComponentTests : CultureIsolatedTest
{
    private static DayDashTestContext NewContext(out FakeCalendarService calendar, out FakeFileShareService share)
    {
        calendar = new FakeCalendarService();
        share = new FakeFileShareService();
        var ctx = new DayDashTestContext();
        ctx.Services.AddSingleton<ICalendarService>(calendar);
        ctx.Services.AddSingleton<IFileShareService>(share);
        return ctx;
    }

    [Fact]
    public void Toggling_the_view_swaps_the_child_component()
    {
        using var ctx = NewContext(out _, out _);
        var cut = ctx.RenderComponent<CalendarComponent>();

        Assert.NotEmpty(cut.FindComponents<CalendarMonthView>());
        Assert.Empty(cut.FindComponents<CalendarWeekView>());

        cut.FindAll(".calendar-view-switch button")[1].Click(); // Week

        Assert.Empty(cut.FindComponents<CalendarMonthView>());
        Assert.NotEmpty(cut.FindComponents<CalendarWeekView>());
    }

    [Fact]
    public void Export_button_calls_the_service_and_the_share_sheet()
    {
        using var ctx = NewContext(out var calendar, out var share);
        var cut = ctx.RenderComponent<CalendarComponent>();

        cut.FindAll(".calendar-actions button")[1].Click(); // Export

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(calendar.LastExportPath);
            Assert.Equal(calendar.LastExportPath, share.LastPath);
            Assert.Contains("exportiert", cut.Markup); // ExportSuccess
        });
    }

    [Fact]
    public void New_event_button_opens_the_edit_form()
    {
        using var ctx = NewContext(out _, out _);
        var cut = ctx.RenderComponent<CalendarComponent>();

        cut.FindAll(".calendar-actions button")[0].Click(); // New event

        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindComponents<EventEditComponent>()));
    }
}
