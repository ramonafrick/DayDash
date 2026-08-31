using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.StudyPlanner;

public class StudyPlannerServiceTests
{
    // Fixture "today" = 2026-03-10.
    private static readonly DateOnly Today = new(2026, 3, 10);

    private static async Task SeedSubjectsAsync(SqliteDbContextFixture f)
        => await SeedingHost.Initializer(f.Context).InitializeAsync();

    [Fact]
    public async Task Recommendation_uses_the_stored_minutes_per_goal()
    {
        await using var f = new SqliteDbContextFixture();
        await SeedSubjectsAsync(f);
        var host = new StudyPlannerHost(f);
        // Bump "Mathematik" to 25 min/goal.
        var math = (await host.SubjectService.GetAllAsync()).Single(s => s.Name == "Mathematik");
        math.MinutesPerGoal = 25;
        await host.SubjectService.SaveAsync(math);

        var recommended = await host.Service.CalculateRecommendedMinutesAsync(3, "Mathematik");

        Assert.Equal(75, recommended);
    }

    [Fact]
    public async Task Recommendation_for_an_unknown_subject_falls_back_to_15()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);

        Assert.Equal(60, await host.Service.CalculateRecommendedMinutesAsync(4, "Astrophysik"));
    }

    [Fact]
    public async Task CreateExam_persists_computed_RecommendedMinutes_and_DailyMinutes()
    {
        await using var f = new SqliteDbContextFixture();
        await SeedSubjectsAsync(f);
        var host = new StudyPlannerHost(f);

        var exam = TestData.AnExam(subject: "Deutsch", examDate: Today.AddDays(4), totalStudyMinutes: 120);
        exam.LearningGoals =
        [
            new LearningGoal { Id = Guid.NewGuid(), Text = "a" },
            new LearningGoal { Id = Guid.NewGuid(), Text = "b" },
        ];

        var id = await host.Service.CreateExamAsync(exam);
        f.Context.ChangeTracker.Clear();
        var saved = await f.Context.Set<Exam>().SingleAsync(e => e.Id == id);

        Assert.Equal(30, saved.RecommendedMinutes); // 2 goals x 15
        Assert.Equal(30, saved.DailyMinutes);       // 120 / 4 days
        Assert.Contains(host.Notifier.Changes, c => c.Kind == DataChangeKind.ExamSaved && c.EntityId == id);
    }

    [Fact]
    public async Task GetTodayStudyPlan_includes_only_open_exams_with_a_daily_split()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        f.Context.AddRange(
            NewExam("today", Today, 30),
            NewExam("yesterday", Today.AddDays(-1), 30),
            NewExam("future no split", Today.AddDays(10), 0),
            NewExam("future with split", Today.AddDays(10), 20));
        await f.Context.SaveChangesAsync();

        var plan = await host.Service.GetTodayStudyPlanAsync();

        Assert.Equal(new[] { "today", "future with split" }, plan.Select(e => e.Title));

        static Exam NewExam(string title, DateOnly date, int daily) => new()
        {
            Id = Guid.NewGuid(), Title = title, Subject = "X", ExamDate = date, DailyMinutes = daily,
        };
    }

    [Fact]
    public async Task DeleteExam_raises_ExamDeleted_and_a_non_existent_id_is_a_no_op()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var id = await host.Service.CreateExamAsync(TestData.AnExam());

        await host.Service.DeleteExamAsync(id);
        await host.Service.DeleteExamAsync(Guid.NewGuid());

        Assert.Single(host.Notifier.Changes, c => c.Kind == DataChangeKind.ExamDeleted);
    }
}
