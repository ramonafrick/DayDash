using Bunit;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.UI.Components;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Localization;

public class FormStatePreservationTests : CultureIsolatedTest
{
    [Fact]
    public void Switching_the_language_keeps_a_half_filled_form_and_only_swaps_labels()
    {
        using var ctx = new DayDashTestContext();
        var cut = ctx.RenderComponent<EventEditComponent>(p => p
            .Add(c => c.Event, (CalendarEvent?)null)
            .Add(c => c.EventTypes, []));

        cut.Find("#ev-title").Change("Meine Prüfung");
        Assert.Contains("Titel", cut.Markup); // de label

        ctx.CultureState.ChangeCulture("en");

        cut.WaitForAssertion(() =>
        {
            Assert.Equal("Meine Prüfung", cut.Find("#ev-title").GetAttribute("value")); // field value survives
            Assert.Contains("Title", cut.Markup);                                        // label is now English
        });
    }
}
