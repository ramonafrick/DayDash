using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.StudyPlanner;

public class LearningGoalPersistenceTests
{
    [Fact]
    public async Task Goals_are_saved_with_contiguous_sort_order()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var exam = TestData.AnExam();
        exam.LearningGoals =
        [
            new LearningGoal { Id = Guid.NewGuid(), Text = "one" },
            new LearningGoal { Id = Guid.NewGuid(), Text = "two" },
            new LearningGoal { Id = Guid.NewGuid(), Text = "three" },
        ];

        var id = await host.Service.CreateExamAsync(exam);
        f.Context.ChangeTracker.Clear();

        var goals = await f.Context.Set<LearningGoal>().Where(g => g.ExamId == id).OrderBy(g => g.SortOrder).ToListAsync();
        Assert.Equal(new[] { 0, 1, 2 }, goals.Select(g => g.SortOrder));
    }

    [Fact]
    public async Task SetGoalChecked_persists_the_single_flag()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var exam = TestData.AnExam();
        var goal = new LearningGoal { Id = Guid.NewGuid(), Text = "study chapter 1" };
        exam.LearningGoals = [goal];
        var id = await host.Service.CreateExamAsync(exam);

        await host.Service.SetGoalCheckedAsync(goal.Id, true);
        f.Context.ChangeTracker.Clear();

        var reloaded = await f.Context.Set<LearningGoal>().SingleAsync(g => g.Id == goal.Id);
        Assert.True(reloaded.IsChecked);
    }

    [Fact]
    public async Task SaveLearningGoals_replaces_the_list_without_duplicating()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var exam = TestData.AnExam();
        exam.LearningGoals = [new LearningGoal { Id = Guid.NewGuid(), Text = "old" }];
        var id = await host.Service.CreateExamAsync(exam);

        await host.Service.SaveLearningGoalsAsync(id,
        [
            new LearningGoal { Text = "new a" },
            new LearningGoal { Text = "new b" },
        ]);
        f.Context.ChangeTracker.Clear();

        var texts = await f.Context.Set<LearningGoal>().Where(g => g.ExamId == id).OrderBy(g => g.SortOrder)
            .Select(g => g.Text).ToListAsync();
        Assert.Equal(new[] { "new a", "new b" }, texts);
    }
}
