using Bunit;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.UI.Components;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Calendar;

public class EventEditComponentTests : CultureIsolatedTest
{
    [Fact]
    public void Empty_title_blocks_submit_and_shows_the_localized_error()
    {
        using var ctx = new DayDashTestContext();
        CalendarEvent? saved = null;
        var cut = ctx.Render<EventEditComponent>(p => p
            .Add(c => c.OnSave, e => saved = e));

        cut.Find("form").Submit();

        Assert.Null(saved);
        Assert.Contains("Titel", cut.Markup); // TitleRequired -> "Bitte einen Titel eingeben" contains "Titel"
    }

    [Fact]
    public void TimeTo_before_TimeFrom_blocks_submit()
    {
        using var ctx = new DayDashTestContext();
        CalendarEvent? saved = null;
        var cut = ctx.Render<EventEditComponent>(p => p
            .Add(c => c.OnSave, e => saved = e));

        cut.Find("#ev-title").Change("Test");
        cut.Find("#ev-from").Change("14:00");
        cut.Find("#ev-to").Change("09:00");
        cut.Find("form").Submit();

        Assert.Null(saved);
        Assert.Contains("Bis muss nach Von liegen", cut.Markup);
    }

    [Fact]
    public void Ticking_all_day_hides_the_time_inputs()
    {
        using var ctx = new DayDashTestContext();
        var cut = ctx.Render<EventEditComponent>();

        Assert.NotEmpty(cut.FindAll("#ev-from"));

        cut.Find("input[type=checkbox]").Change(true);

        Assert.Empty(cut.FindAll("#ev-from"));
    }

    [Fact]
    public void A_valid_all_day_event_saves_with_null_times()
    {
        using var ctx = new DayDashTestContext();
        CalendarEvent? saved = null;
        var cut = ctx.Render<EventEditComponent>(p => p
            .Add(c => c.OnSave, e => saved = e));

        cut.Find("#ev-title").Change("Ferien");
        cut.Find("input[type=checkbox]").Change(true);
        cut.Find("form").Submit();

        Assert.NotNull(saved);
        Assert.True(saved!.IsAllDay);
        Assert.Null(saved.TimeFrom);
        Assert.Null(saved.TimeTo);
        Assert.Equal("Ferien", saved.Title);
    }
}
