using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Infrastructure;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.StudyPlanner;

public class ExamRepositoryTests
{
    // Fixture "now" is 2026-03-10.
    private static readonly DateOnly Today = new(2026, 3, 10);

    private static ExamRepository Repo(SqliteDbContextFixture f) => new(f.Context, f.Time);

    [Fact]
    public async Task GetUpcoming_is_a_date_window_not_a_row_count()
    {
        await using var f = new SqliteDbContextFixture();
        for (var i = 0; i < 20; i++)
        {
            f.Context.Add(TestData.AnExam(examDate: Today.AddDays(3)));  // 20 exams, all within 7 days
        }

        await f.Context.SaveChangesAsync();

        var result = await Repo(f).GetUpcomingAsync(7);

        Assert.Equal(20, result.Count);
    }

    [Fact]
    public async Task GetUpcoming_includes_today_and_excludes_yesterday()
    {
        await using var f = new SqliteDbContextFixture();
        f.Context.Add(TestData.AnExam(title: "today", examDate: Today));
        f.Context.Add(TestData.AnExam(title: "yesterday", examDate: Today.AddDays(-1)));
        f.Context.Add(TestData.AnExam(title: "next week", examDate: Today.AddDays(6)));
        f.Context.Add(TestData.AnExam(title: "next month", examDate: Today.AddDays(40)));
        await f.Context.SaveChangesAsync();

        var titles = (await Repo(f).GetUpcomingAsync(7)).Select(e => e.Title).ToArray();

        Assert.Equal(new[] { "today", "next week" }, titles);
    }

    [Fact]
    public async Task GetWithGoals_returns_goals_ordered_by_sort_order()
    {
        await using var f = new SqliteDbContextFixture();
        var exam = TestData.AnExam();
        exam.LearningGoals =
        [
            new LearningGoal { Id = Guid.NewGuid(), Text = "c", SortOrder = 2 },
            new LearningGoal { Id = Guid.NewGuid(), Text = "a", SortOrder = 0 },
            new LearningGoal { Id = Guid.NewGuid(), Text = "b", SortOrder = 1 },
        ];
        f.Context.Add(exam);
        await f.Context.SaveChangesAsync();
        f.Context.ChangeTracker.Clear();

        var loaded = await Repo(f).GetWithGoalsAsync(exam.Id);

        Assert.NotNull(loaded);
        Assert.Equal(new[] { "a", "b", "c" }, loaded!.LearningGoals.Select(g => g.Text));
    }
}
