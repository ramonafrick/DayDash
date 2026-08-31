using System.Globalization;
using DayDash.Modules.Settings.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Settings;

public class CultureStateServiceTests : CultureIsolatedTest
{
    private static CultureStateService NewService() => new(NullLogger<CultureStateService>.Instance);

    [Fact]
    public void ChangeCulture_sets_all_four_culture_statics()
    {
        var service = NewService();
        var target = new CultureInfo("en");

        service.ChangeCulture(target);

        Assert.Equal("en", CultureInfo.DefaultThreadCurrentCulture?.Name);
        Assert.Equal("en", CultureInfo.DefaultThreadCurrentUICulture?.Name);
        Assert.Equal("en", CultureInfo.CurrentCulture.Name);
        Assert.Equal("en", CultureInfo.CurrentUICulture.Name);
        Assert.Equal("en", service.CurrentCultureName);
    }

    [Fact]
    public void ChangeCulture_raises_CultureChanged_once()
    {
        var service = NewService();
        service.ChangeCulture(new CultureInfo("de-CH"));
        var raised = 0;
        service.CultureChanged += (_, _) => raised++;

        service.ChangeCulture(new CultureInfo("en"));

        Assert.Equal(1, raised);
    }

    [Fact]
    public void ChangeCulture_is_noop_for_the_same_culture_name()
    {
        var service = NewService();
        service.ChangeCulture(new CultureInfo("en"));
        var raised = 0;
        service.CultureChanged += (_, _) => raised++;

        service.ChangeCulture(new CultureInfo("en"));

        Assert.Equal(0, raised);
    }

    [Fact]
    public void ChangeCulture_notifies_all_subscribers()
    {
        var service = NewService();
        service.ChangeCulture(new CultureInfo("de-CH"));
        var a = 0;
        var b = 0;
        service.CultureChanged += (_, _) => a++;
        service.CultureChanged += (_, _) => b++;

        service.ChangeCulture(new CultureInfo("en"));

        Assert.Equal(1, a);
        Assert.Equal(1, b);
    }

    [Theory]
    [InlineData(null, "de-CH")]
    [InlineData("", "de-CH")]
    [InlineData("de", "de-CH")]
    [InlineData("de-DE", "de-CH")]
    [InlineData("fr-FR", "de-CH")]
    [InlineData("en", "en")]
    [InlineData("en-US", "en")]
    public void SupportedCultures_Normalize_maps_to_the_two_shipped_cultures(string? input, string expected)
        => Assert.Equal(expected, SupportedCultures.Normalize(input));
}
