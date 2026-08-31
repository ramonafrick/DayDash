using DayDash.Modules.Settings.Application.Services;
using Xunit;

namespace DayDash.Tests.Localization;

public class CultureStartupTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("fr-CH")]
    [InlineData("de")]
    [InlineData("de-DE")]
    [InlineData("de-CH")]
    [InlineData("garbage!!")]
    public void Unknown_or_german_preferences_resolve_to_the_de_CH_default(string? stored)
    {
        Assert.Equal("de-CH", SupportedCultures.Normalize(stored));
        Assert.Equal("de-CH", SupportedCultures.Resolve(stored).Name);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("en-US")]
    [InlineData("EN-GB")]
    public void Any_english_preference_resolves_to_en(string stored)
    {
        Assert.Equal("en", SupportedCultures.Normalize(stored));
        Assert.Equal("en", SupportedCultures.Resolve(stored).Name);
    }

    [Fact]
    public void Only_de_CH_and_en_are_shipped()
    {
        Assert.Equal(["de-CH", "en"], SupportedCultures.All);
    }
}
