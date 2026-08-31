using DayDash.Modules.Reminder.Infrastructure;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Reminder;

public class ReminderRescheduleHandlerTests : CultureIsolatedTest
{
    private static Exam StudyExam(int dailyMinutes) => new()
    {
        Id = Guid.NewGuid(), Title = "Mathe", Subject = "Mathematik",
        ExamDate = new DateOnly(2026, 3, 20), DailyMinutes = dailyMinutes,
    };

    [Fact]
    public async Task Any_data_change_triggers_a_full_reschedule()
    {
        var host = new ReminderHost();
        var handler = new ReminderRescheduleHandler(host.Service);
        host.StudyPlanner.Exams.Add(StudyExam(30));

        await handler.HandleAsync(new DataChange(DataChangeKind.ExamSaved, Guid.NewGuid()));

        Assert.Equal(1, host.Scheduler.CancelAllCount);
        Assert.NotNull(host.Scheduler.Daily);
    }

    [Fact]
    public async Task Deleting_the_last_study_exam_clears_the_daily_reminder()
    {
        var host = new ReminderHost();
        var handler = new ReminderRescheduleHandler(host.Service);
        var exam = StudyExam(30);
        host.StudyPlanner.Exams.Add(exam);
        await handler.HandleAsync(new DataChange(DataChangeKind.ExamSaved, exam.Id));
        Assert.NotNull(host.Scheduler.Daily);

        host.StudyPlanner.Exams.Clear();
        await handler.HandleAsync(new DataChange(DataChangeKind.ExamDeleted, exam.Id));

        Assert.Null(host.Scheduler.Daily);
        Assert.Empty(host.Scheduler.Scheduled);
    }
}
