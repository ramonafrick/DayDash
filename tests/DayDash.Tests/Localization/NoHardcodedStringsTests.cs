using System.Text.RegularExpressions;
using Xunit;

namespace DayDash.Tests.Localization;

/// <summary>
/// No user-facing literal text may live in a module's <c>.razor</c> markup - every visible
/// string goes through <c>IStringLocalizer</c>. Allowed: Material Symbols icon ligatures
/// (text inside a <c>material-symbols</c> span), pure punctuation/number nodes, and anything
/// that is a Razor expression (<c>@...</c>).
/// </summary>
public partial class NoHardcodedStringsTests
{
    [Fact]
    public void Module_razor_markup_contains_no_hardcoded_visible_text()
    {
        var modules = Path.Combine(RepoRoot(), "src", "Modules");
        var offenders = new List<string>();

        foreach (var file in Directory.GetFiles(modules, "*.razor", SearchOption.AllDirectories))
        {
            if (file.Contains("_Imports", StringComparison.Ordinal))
            {
                continue;
            }

            var markup = Sanitize(File.ReadAllText(file));
            var relative = Path.GetRelativePath(RepoRoot(), file);

            foreach (Match m in TextNode().Matches(markup))
            {
                var text = m.Groups[1].Value.Trim();

                // Real UI text starts with a letter and contains no code punctuation; this
                // filters Razor control flow ('}', 'else', '@if (...)'), attributes and exprs.
                if (!LooksLikeProse().IsMatch(text))
                {
                    continue;
                }

                var lineStart = markup.LastIndexOf('\n', m.Index) + 1;
                var newline = markup.IndexOf('\n', m.Index);
                var line = markup[lineStart..(newline < 0 ? markup.Length : newline)];
                if (line.Contains("material-symbols", StringComparison.Ordinal))
                {
                    continue;
                }

                offenders.Add($"{relative}: \"{text}\"");
            }
        }

        Assert.True(offenders.Count == 0, "Hardcoded markup text found:\n" + string.Join("\n", offenders));
    }

    private static string Sanitize(string source)
    {
        source = RazorComment().Replace(source, string.Empty);

        // Cut the trailing @code / @functions block first (before it can be mistaken for a directive line).
        var codeIndex = source.IndexOf("@code", StringComparison.Ordinal);
        if (codeIndex < 0)
        {
            codeIndex = source.IndexOf("@functions", StringComparison.Ordinal);
        }

        if (codeIndex >= 0)
        {
            source = source[..codeIndex];
        }

        // Drop directive lines (@inject / @inherits / @page / @using ...).
        var lines = source.Split('\n')
            .Where(l => !l.TrimStart().StartsWith('@') || l.TrimStart().StartsWith("@("));

        return string.Join('\n', lines);
    }

    private static string RepoRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !Directory.Exists(Path.Combine(dir.FullName, "src", "Modules")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Could not locate the repository root.");
    }

    [GeneratedRegex(@">([^<>]+)<")]
    private static partial Regex TextNode();

    // Starts with a (Unicode) letter, no code punctuation anywhere, at least three chars.
    [GeneratedRegex(@"^\p{L}[^{}()=;_@""`\[\]]{2,}$", RegexOptions.Singleline)]
    private static partial Regex LooksLikeProse();

    [GeneratedRegex(@"@\*.*?\*@", RegexOptions.Singleline)]
    private static partial Regex RazorComment();
}
