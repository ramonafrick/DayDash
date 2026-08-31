using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Reminder.Application.Models;
using DayDash.Modules.Reminder.Application.Services;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Reminder;

public class EventReminderScheduleTests : CultureIsolatedTest
{
    // ReminderHost default "now" is 2026-03-10 08:00Z; config default lead time is 1 day.

    private static NotificationRequest? EventReminder(ReminderHost host, CalendarEvent e)
        => host.Scheduler.Scheduled.FirstOrDefault(r => r.Id == NotificationIds.ForEvent(e.Id));

    [Fact]
    public async Task Default_lead_fires_the_day_before_at_the_event_start_time()
    {
        var host = new ReminderHost();
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 20), from: new TimeOnly(9, 0));
        host.Calendar.Events.Add(e);

        await host.Service.RescheduleAllAsync();

        var reminder = EventReminder(host, e);
        Assert.NotNull(reminder);
        Assert.Equal(new DateTime(2026, 3, 19, 9, 0, 0), reminder!.DeliverAt);
    }

    [Fact]
    public async Task A_per_event_override_wins_over_the_config_default()
    {
        var host = new ReminderHost();
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 20), from: new TimeOnly(9, 0));
        e.ReminderDaysBefore = 3;
        host.Calendar.Events.Add(e);

        await host.Service.RescheduleAllAsync();

        Assert.Equal(new DateOnly(2026, 3, 17), DateOnly.FromDateTime(EventReminder(host, e)!.DeliverAt.Date));
    }

    [Fact]
    public async Task A_reminder_whose_moment_has_already_passed_is_not_scheduled()
    {
        var host = new ReminderHost();
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 10), from: new TimeOnly(9, 0)); // lead 1 -> 03-09 09:00, in the past
        host.Calendar.Events.Add(e);

        await host.Service.RescheduleAllAsync();

        Assert.Null(EventReminder(host, e));
    }

    [Fact]
    public async Task Lead_zero_fires_on_the_event_day()
    {
        var host = new ReminderHost();
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 15), from: new TimeOnly(10, 0));
        e.ReminderDaysBefore = 0;
        host.Calendar.Events.Add(e);

        await host.Service.RescheduleAllAsync();

        Assert.Equal(new DateTime(2026, 3, 15, 10, 0, 0), EventReminder(host, e)!.DeliverAt);
    }

    [Fact]
    public async Task An_all_day_event_reminder_falls_back_to_eight_in_the_morning()
    {
        var host = new ReminderHost();
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 20), allDay: true);
        host.Calendar.Events.Add(e);

        await host.Service.RescheduleAllAsync();

        Assert.Equal(new DateTime(2026, 3, 19, 8, 0, 0), EventReminder(host, e)!.DeliverAt);
    }

    [Fact]
    public async Task Events_beyond_the_scheduling_window_are_ignored()
    {
        var host = new ReminderHost();
        var e = TestData.AnEvent(date: new DateOnly(2026, 6, 1), from: new TimeOnly(9, 0));
        host.Calendar.Events.Add(e);

        await host.Service.RescheduleAllAsync();

        Assert.Null(EventReminder(host, e));
    }
}
