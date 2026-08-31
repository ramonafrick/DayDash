using DayDash.Modules.Reminder.Application.Services;
using DayDash.Modules.Reminder.Domain;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Reminder;

public class ReminderServiceTests : CultureIsolatedTest
{
    private static Exam StudyExam(int dailyMinutes) => new()
    {
        Id = Guid.NewGuid(), Title = "Mathe", Subject = "Mathematik",
        ExamDate = new DateOnly(2026, 3, 20), DailyMinutes = dailyMinutes,
    };

    [Fact]
    public async Task Nothing_to_study_schedules_no_daily_reminder()
    {
        var host = new ReminderHost();

        await host.Service.RescheduleAllAsync();

        Assert.Equal(1, host.Scheduler.CancelAllCount);
        Assert.Null(host.Scheduler.Daily);
        Assert.Empty(host.Scheduler.Scheduled);
    }

    [Fact]
    public async Task A_study_load_schedules_exactly_one_repeating_daily_reminder_at_the_configured_time()
    {
        var host = new ReminderHost();
        host.StudyPlanner.Exams.Add(StudyExam(30));

        await host.Service.RescheduleAllAsync();

        var daily = Assert.Single(host.Scheduler.Scheduled);
        Assert.Equal(NotificationIds.DailyStudyReminder, daily.Id);
        Assert.True(daily.RepeatDaily);
        Assert.Equal(new TimeSpan(15, 30, 0), daily.DeliverAt.TimeOfDay);
        Assert.Equal(new DateOnly(2026, 3, 10), DateOnly.FromDateTime(daily.DeliverAt.Date));
    }

    [Fact]
    public async Task The_daily_reminder_rolls_to_tomorrow_when_its_time_already_passed_today()
    {
        var host = new ReminderHost(new DateTimeOffset(2026, 3, 10, 16, 0, 0, TimeSpan.Zero));
        host.StudyPlanner.Exams.Add(StudyExam(30));

        await host.Service.RescheduleAllAsync();

        Assert.Equal(new DateOnly(2026, 3, 11), DateOnly.FromDateTime(host.Scheduler.Daily!.DeliverAt.Date));
    }

    [Fact]
    public async Task Disabled_config_cancels_everything_and_schedules_nothing()
    {
        var host = new ReminderHost();
        host.Config.Config = new ReminderConfig { IsEnabled = false };
        host.StudyPlanner.Exams.Add(StudyExam(30));
        host.Calendar.Events.Add(TestData.AnEvent(date: new DateOnly(2026, 3, 15)));

        await host.Service.RescheduleAllAsync();

        Assert.Equal(1, host.Scheduler.CancelAllCount);
        Assert.Empty(host.Scheduler.Scheduled);
    }

    [Fact]
    public async Task SaveConfig_persists_the_new_values_and_reschedules()
    {
        var host = new ReminderHost();
        host.StudyPlanner.Exams.Add(StudyExam(30));

        await host.Service.SaveConfigAsync(new ReminderConfig
        {
            DailyStudyReminderTime = new TimeOnly(9, 0),
            EventReminderDaysBefore = 2,
            IsEnabled = true,
        });

        Assert.Equal(1, host.Config.Saves);
        Assert.Equal(new TimeOnly(9, 0), host.Config.Config.DailyStudyReminderTime);
        Assert.Equal(new TimeSpan(9, 0, 0), host.Scheduler.Daily!.DeliverAt.TimeOfDay);
    }

    [Fact]
    public async Task Changing_the_time_and_rescheduling_replaces_the_daily_reminder()
    {
        var host = new ReminderHost();
        host.StudyPlanner.Exams.Add(StudyExam(30));
        await host.Service.RescheduleAllAsync();

        host.Config.Config = new ReminderConfig { DailyStudyReminderTime = new TimeOnly(9, 0), IsEnabled = true };
        await host.Service.RescheduleAllAsync();

        Assert.Single(host.Scheduler.Scheduled);
        Assert.Equal(new TimeSpan(9, 0, 0), host.Scheduler.Daily!.DeliverAt.TimeOfDay);
        Assert.Equal(2, host.Scheduler.CancelAllCount);
    }
}
