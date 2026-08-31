using DayDash.Modules.Camera.Application.Services;
using Xunit;

namespace DayDash.Tests.Camera;

public class LearningGoalParserTests
{
    private readonly LearningGoalParser _parser = new();

    [Fact]
    public void Each_line_becomes_a_goal_with_contiguous_sort_order()
    {
        var examId = Guid.NewGuid();

        var goals = _parser.ParseToLearningGoals("Kapitel 1 lernen\nVokabeln üben\nZusammenfassung schreiben", examId);

        Assert.Equal(3, goals.Count);
        Assert.Equal([0, 1, 2], goals.Select(g => g.SortOrder));
        Assert.All(goals, g => Assert.False(g.IsChecked));
        Assert.All(goals, g => Assert.Equal(examId, g.ExamId));
        Assert.All(goals, g => Assert.NotEqual(Guid.Empty, g.Id));
    }

    [Fact]
    public void Splits_on_both_crlf_and_lf()
    {
        var goals = _parser.ParseToLearningGoals("one\r\ntwo\nthree", Guid.NewGuid());

        Assert.Equal(["one", "two", "three"], goals.Select(g => g.Text));
    }

    [Fact]
    public void Blank_and_whitespace_only_lines_are_dropped_without_leaving_gaps()
    {
        var goals = _parser.ParseToLearningGoals("first\n\n   \n\t\nsecond", Guid.NewGuid());

        Assert.Equal(["first", "second"], goals.Select(g => g.Text));
        Assert.Equal([0, 1], goals.Select(g => g.SortOrder));
    }

    [Fact]
    public void Empty_input_yields_no_goals()
    {
        Assert.Empty(_parser.ParseToLearningGoals(string.Empty, Guid.NewGuid()));
    }

    [Fact]
    public void Whitespace_only_input_yields_no_goals()
    {
        Assert.Empty(_parser.ParseToLearningGoals("   \n\t \r\n  ", Guid.NewGuid()));
    }

    [Fact]
    public void Long_lines_are_capped_at_200_characters()
    {
        var line = new string('x', 500);

        var goals = _parser.ParseToLearningGoals(line, Guid.NewGuid());

        Assert.Single(goals);
        Assert.Equal(200, goals[0].Text.Length);
    }

    [Fact]
    public void Truncation_never_splits_a_surrogate_pair()
    {
        var line = new string('x', LearningGoalParser.MaxLength - 1) + "\U0001F600"; // 199 x + emoji (2 code units)

        var text = Assert.Single(_parser.ParseToLearningGoals(line, Guid.NewGuid())).Text;

        Assert.Equal(LearningGoalParser.MaxLength - 1, text.Length);
        Assert.False(char.IsHighSurrogate(text[^1]));
    }

    [Fact]
    public void Leading_trailing_whitespace_is_trimmed_and_internal_runs_collapsed()
    {
        var goals = _parser.ParseToLearningGoals("   spaced    out   words   ", Guid.NewGuid());

        Assert.Equal("spaced out words", Assert.Single(goals).Text);
    }
}
