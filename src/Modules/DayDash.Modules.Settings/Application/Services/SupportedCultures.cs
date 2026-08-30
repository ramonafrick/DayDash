using System.Globalization;

namespace DayDash.Modules.Settings.Application.Services;

/// <summary>
/// The two cultures DayDash ships (Requirements.md §3). <c>de-CH</c> is the neutral/default;
/// <c>en</c> is the only satellite.
/// </summary>
public static class SupportedCultures
{
    public const string Default = "de-CH";
    public const string English = "en";

    public static readonly IReadOnlyList<string> All = [Default, English];

    public static string Normalize(string? cultureName)
    {
        if (string.IsNullOrWhiteSpace(cultureName))
        {
            return Default;
        }

        // Accept "de", "de-CH", "de-DE" ... -> de-CH; anything starting with "en" -> en.
        if (cultureName.StartsWith("en", StringComparison.OrdinalIgnoreCase))
        {
            return English;
        }

        return Default;
    }

    public static CultureInfo Resolve(string? cultureName) => new(Normalize(cultureName));
}
