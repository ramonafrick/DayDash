using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.StudyPlanner;

public class SubjectConfigServiceTests
{
    [Fact]
    public async Task Add_rename_and_delete_round_trip_through_the_database()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var subject = new SubjectConfig { Id = Guid.NewGuid(), Name = "Musik", MinutesPerGoal = 10 };

        await host.SubjectService.SaveAsync(subject);
        Assert.Equal("Musik", (await host.SubjectService.GetAllAsync()).Single().Name);

        subject.Name = "Musikunterricht";
        subject.MinutesPerGoal = 12;
        await host.SubjectService.SaveAsync(subject);
        var renamed = (await host.SubjectService.GetAllAsync()).Single();
        Assert.Equal("Musikunterricht", renamed.Name);
        Assert.Equal(12, renamed.MinutesPerGoal);

        await host.SubjectService.DeleteAsync(subject.Id);
        Assert.Empty(await host.SubjectService.GetAllAsync());
    }

    [Fact]
    public async Task Deleting_a_subject_leaves_exams_that_reference_it_by_name_intact()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var subject = new SubjectConfig { Id = Guid.NewGuid(), Name = "Chemie", MinutesPerGoal = 15 };
        await host.SubjectService.SaveAsync(subject);
        var examId = await host.Service.CreateExamAsync(TestData.AnExam(subject: "Chemie"));

        await host.SubjectService.DeleteAsync(subject.Id);
        f.Context.ChangeTracker.Clear();

        var exam = await f.Context.Set<Exam>().SingleAsync(e => e.Id == examId);
        Assert.Equal("Chemie", exam.Subject);
        Assert.Equal(15, await host.Service.CalculateRecommendedMinutesAsync(1, "Chemie")); // fallback rate
    }

    [Fact]
    public async Task Save_and_delete_raise_SubjectConfigChanged()
    {
        await using var f = new SqliteDbContextFixture();
        var host = new StudyPlannerHost(f);
        var subject = new SubjectConfig { Id = Guid.NewGuid(), Name = "Sport", MinutesPerGoal = 15 };

        await host.SubjectService.SaveAsync(subject);
        await host.SubjectService.DeleteAsync(subject.Id);

        Assert.Equal(2, host.Notifier.Changes.Count(c => c.Kind == DataChangeKind.SubjectConfigChanged));
    }
}
