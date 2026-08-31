using Bunit;
using DayDash.Modules.Camera.UI.Components;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Camera;

public class LearningGoalEditComponentTests : CultureIsolatedTest
{
    private static List<LearningGoal> ThreeGoals() =>
    [
        new() { Id = Guid.NewGuid(), Text = "a", SortOrder = 0 },
        new() { Id = Guid.NewGuid(), Text = "b", SortOrder = 1 },
        new() { Id = Guid.NewGuid(), Text = "c", SortOrder = 2 },
    ];

    [Fact]
    public void Deleting_a_row_renumbers_the_rest_contiguously()
    {
        using var ctx = new DayDashTestContext();
        var goals = ThreeGoals();
        var cut = ctx.Render<LearningGoalEditComponent>(p => p.Add(c => c.Goals, goals));

        cut.FindAll("button").Where(b => b.TextContent.Contains("Zeile löschen")).ElementAt(1).Click();

        Assert.Equal(["a", "c"], goals.Select(g => g.Text));
        Assert.Equal([0, 1], goals.Select(g => g.SortOrder));
    }

    [Fact]
    public void Move_up_on_the_first_row_is_a_no_op()
    {
        using var ctx = new DayDashTestContext();
        var goals = ThreeGoals();
        var cut = ctx.Render<LearningGoalEditComponent>(p => p.Add(c => c.Goals, goals));

        cut.FindAll("button").First(b => b.TextContent.Contains("Nach oben")).Click();

        Assert.Equal(["a", "b", "c"], goals.Select(g => g.Text));
    }

    [Fact]
    public void Move_down_on_the_last_row_is_a_no_op()
    {
        using var ctx = new DayDashTestContext();
        var goals = ThreeGoals();
        var cut = ctx.Render<LearningGoalEditComponent>(p => p.Add(c => c.Goals, goals));

        cut.FindAll("button").Where(b => b.TextContent.Contains("Nach unten")).Last().Click();

        Assert.Equal(["a", "b", "c"], goals.Select(g => g.Text));
    }

    [Fact]
    public void Move_up_swaps_with_the_previous_row_and_renumbers()
    {
        using var ctx = new DayDashTestContext();
        var goals = ThreeGoals();
        var cut = ctx.Render<LearningGoalEditComponent>(p => p.Add(c => c.Goals, goals));

        cut.FindAll("button").Where(b => b.TextContent.Contains("Nach oben")).ElementAt(2).Click();

        Assert.Equal(["a", "c", "b"], goals.Select(g => g.Text));
        Assert.Equal([0, 1, 2], goals.Select(g => g.SortOrder));
    }
}
