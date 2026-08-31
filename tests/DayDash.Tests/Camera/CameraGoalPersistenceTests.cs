using DayDash.Modules.Camera.Application.Services;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.Camera;

/// <summary>The Camera OCR path: parsed lines land as real <see cref="LearningGoal"/> rows on an exam.</summary>
public class CameraGoalPersistenceTests
{
    private readonly LearningGoalParser _parser = new();

    [Fact]
    public async Task Parsed_goals_are_persisted_against_the_selected_exam()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var id = await host.Service.CreateExamAsync(TestData.AnExam());
        f.Context.ChangeTracker.Clear();

        var goals = _parser.ParseToLearningGoals("Lernziel A\nLernziel B\nLernziel C\nLernziel D", id);
        await host.Service.SaveLearningGoalsAsync(id, goals);
        f.Context.ChangeTracker.Clear();

        var stored = await f.Context.Set<LearningGoal>()
            .Where(g => g.ExamId == id).OrderBy(g => g.SortOrder).ToListAsync();
        Assert.Equal(["Lernziel A", "Lernziel B", "Lernziel C", "Lernziel D"], stored.Select(g => g.Text));
        Assert.All(stored, g => Assert.Equal(id, g.ExamId));
    }

    [Fact]
    public async Task Re_scanning_replaces_the_goal_list_instead_of_duplicating()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var id = await host.Service.CreateExamAsync(TestData.AnExam());
        f.Context.ChangeTracker.Clear();

        await host.Service.SaveLearningGoalsAsync(id, _parser.ParseToLearningGoals("old one\nold two", id));
        f.Context.ChangeTracker.Clear();
        await host.Service.SaveLearningGoalsAsync(id, _parser.ParseToLearningGoals("fresh", id));
        f.Context.ChangeTracker.Clear();

        var stored = await f.Context.Set<LearningGoal>().Where(g => g.ExamId == id).ToListAsync();
        Assert.Equal(["fresh"], stored.Select(g => g.Text));
    }
}
