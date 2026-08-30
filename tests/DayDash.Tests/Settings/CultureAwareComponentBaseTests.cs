using System.Globalization;
using Bunit;
using DayDash.Modules.Settings.UI;
using DayDash.Tests.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Xunit;

namespace DayDash.Tests.Settings;

public class CultureAwareComponentBaseTests
{
    private sealed class Probe : CultureAwareComponentBase
    {
        public int RenderCount { get; private set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            RenderCount++;
            builder.AddContent(0, CultureState.CurrentCultureName);
        }
    }

    [Fact]
    public void Re_renders_when_the_culture_changes()
    {
        using var ctx = new DayDashTestContext();
        ctx.CultureState.ChangeCulture(new CultureInfo("de-CH"));
        var cut = ctx.RenderComponent<Probe>();
        var before = cut.Instance.RenderCount;

        ctx.CultureState.ChangeCulture(new CultureInfo("en"));

        cut.WaitForAssertion(() => Assert.True(cut.Instance.RenderCount > before));
    }

    [Fact]
    public void Unsubscribes_on_dispose()
    {
        using var ctx = new DayDashTestContext();
        ctx.CultureState.ChangeCulture(new CultureInfo("de-CH"));
        var cut = ctx.RenderComponent<Probe>();
        var probe = cut.Instance;

        cut.Instance.Dispose();
        var after = probe.RenderCount;

        ctx.CultureState.ChangeCulture(new CultureInfo("en"));

        Assert.Equal(after, probe.RenderCount);
    }
}
