using System.Xml.Linq;
using Xunit;

namespace DayDash.Tests.Localization;

/// <summary>
/// Every neutral (de-CH) <c>*.resx</c> under <c>src/Modules</c> must have a matching
/// <c>*.en.resx</c> with the exact same set of keys and no empty values. This is the
/// enforcement mechanism for the Slice 7 zero-hardcoded-strings / resx-completeness audit.
/// </summary>
public class ResourceParityTests
{
    public static IEnumerable<object[]> NeutralResxFiles()
    {
        var modules = Path.Combine(RepoRoot(), "src", "Modules");
        foreach (var file in Directory.GetFiles(modules, "*.resx", SearchOption.AllDirectories))
        {
            var name = Path.GetFileName(file);
            // Skip satellites like Foo.en.resx (two-letter culture segment before .resx).
            var segments = name.Split('.');
            if (segments.Length >= 3 && segments[^2].Length == 2)
            {
                continue;
            }

            yield return [Path.GetRelativePath(RepoRoot(), file)];
        }
    }

    [Theory]
    [MemberData(nameof(NeutralResxFiles))]
    public void Neutral_and_english_resources_have_identical_keys_and_no_empty_values(string relativeNeutralPath)
    {
        var neutralPath = Path.Combine(RepoRoot(), relativeNeutralPath);
        var englishPath = neutralPath[..^".resx".Length] + ".en.resx";

        Assert.True(File.Exists(englishPath), $"Missing English satellite for {relativeNeutralPath}");

        var neutral = ReadEntries(neutralPath);
        var english = ReadEntries(englishPath);

        Assert.Empty(neutral.Where(e => string.IsNullOrWhiteSpace(e.Value)).Select(e => e.Key));
        Assert.Empty(english.Where(e => string.IsNullOrWhiteSpace(e.Value)).Select(e => e.Key));

        var neutralKeys = neutral.Keys.OrderBy(k => k).ToArray();
        var englishKeys = english.Keys.OrderBy(k => k).ToArray();

        Assert.Equal(neutralKeys, englishKeys);
    }

    private static Dictionary<string, string> ReadEntries(string path) =>
        XDocument.Load(path).Root!
            .Elements("data")
            .ToDictionary(
                d => (string)d.Attribute("name")!,
                d => (string?)d.Element("value") ?? string.Empty);

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "src", "Modules")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Could not locate the repository root.");
    }
}
