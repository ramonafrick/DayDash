using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.Storage;

public class ReferentialIntegrityTests
{
    [Fact]
    public async Task Deleting_an_exam_cascades_its_learning_goals()
    {
        await using var f = new SqliteDbContextFixture();
        var exam = TestData.AnExam();
        exam.LearningGoals =
        [
            new LearningGoal { Id = Guid.NewGuid(), Text = "a", SortOrder = 0 },
            new LearningGoal { Id = Guid.NewGuid(), Text = "b", SortOrder = 1 },
        ];
        f.Context.Add(exam);
        await f.Context.SaveChangesAsync();

        f.Context.Remove(exam);
        await f.Context.SaveChangesAsync();

        Assert.Equal(0, await f.Context.Set<LearningGoal>().CountAsync());
    }

    [Fact]
    public async Task Deleting_an_event_type_nulls_the_link_but_keeps_the_event()
    {
        await using var f = new SqliteDbContextFixture();
        var type = new EventTypeConfig { Id = Guid.NewGuid(), Key = string.Empty, Name = "Custom", Color = "#111111" };
        var e = TestData.AnEvent();
        e.EventType = type;
        f.Context.Add(e);
        await f.Context.SaveChangesAsync();

        f.Context.Remove(type);
        await f.Context.SaveChangesAsync();
        f.Context.ChangeTracker.Clear();

        var reloaded = await f.Context.Set<CalendarEvent>().SingleAsync(x => x.Id == e.Id);
        Assert.Null(reloaded.EventTypeId);
        Assert.Equal(1, await f.Context.Set<CalendarEvent>().CountAsync());
    }
}
