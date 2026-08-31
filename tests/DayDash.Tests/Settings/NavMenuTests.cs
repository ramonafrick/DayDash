using System.Globalization;
using Bunit;
using DayDash.Modules.Settings.UI.Layout;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Settings;

public class NavMenuTests : CultureIsolatedTest
{
    [Fact]
    public void Renders_the_four_DayDash_nav_links()
    {
        using var ctx = new DayDashTestContext();

        var cut = ctx.RenderComponent<NavMenu>();

        var hrefs = cut.FindAll("nav a.nav-link").Select(a => a.GetAttribute("href")).ToArray();
        Assert.Equal(new[] { "calendar", "study", "camera", "settings" }, hrefs);
    }

    [Fact]
    public void Labels_come_from_resources_and_flip_with_the_culture()
    {
        using var ctx = new DayDashTestContext();
        ctx.CultureState.ChangeCulture(new CultureInfo("de-CH"));

        var cut = ctx.RenderComponent<NavMenu>();
        Assert.Contains("Kalender", cut.Markup);
        Assert.Contains("Einstellungen", cut.Markup);

        ctx.CultureState.ChangeCulture(new CultureInfo("en"));
        cut.WaitForAssertion(() =>
        {
            Assert.Contains("Calendar", cut.Markup);
            Assert.Contains("Settings", cut.Markup);
            Assert.DoesNotContain("Kalender", cut.Markup);
        });
    }
}
