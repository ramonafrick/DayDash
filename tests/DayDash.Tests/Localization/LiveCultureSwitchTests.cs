using Bunit;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.UI.Components;
using DayDash.Modules.Settings.UI.Layout;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Localization;

public class LiveCultureSwitchTests : CultureIsolatedTest
{
    [Fact]
    public void NavMenu_labels_flip_when_the_culture_changes_without_a_manual_rerender()
    {
        using var ctx = new DayDashTestContext();

        var cut = ctx.RenderComponent<NavMenu>();
        Assert.Contains("Kalender", cut.Markup);
        Assert.DoesNotContain("Calendar", cut.Markup);

        ctx.CultureState.ChangeCulture("en");

        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Calendar", cut.Markup);
            Assert.DoesNotContain("Kalender", cut.Markup);
        });
    }

    [Fact]
    public void A_feature_component_also_re_renders_its_labels_live()
    {
        using var ctx = new DayDashTestContext();

        var cut = ctx.RenderComponent<EventEditComponent>(p => p
            .Add(c => c.Event, (CalendarEvent?)null)
            .Add(c => c.EventTypes, []));
        Assert.Contains("Speichern", cut.Markup);

        ctx.CultureState.ChangeCulture("en");

        cut.WaitForAssertion(() => Assert.Contains("Save", cut.Markup));
    }
}
