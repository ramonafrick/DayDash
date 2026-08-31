using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.Widget.Application.Services;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Widget;

public class WidgetDataServiceTests
{
    // SqliteDbContextFixture pins "now" to Tuesday 2026-03-10 08:00.

    private static CalendarEvent Event(string title, DateOnly date, TimeOnly? from = null, bool allDay = false) => new()
    {
        Id = Guid.NewGuid(), Title = title, Date = date, TimeFrom = from, IsAllDay = allDay,
    };

    private static Exam Exam(string subject, DateOnly examDate, int dailyMinutes) => new()
    {
        Id = Guid.NewGuid(), Title = subject, Subject = subject, ExamDate = examDate, DailyMinutes = dailyMinutes,
    };

    [Fact]
    public async Task Day_lists_todays_events_all_day_first_then_by_time()
    {
        await using var f = new SqliteDbContextFixture();
        f.Context.AddRange(
            Event("Training", new DateOnly(2026, 3, 10), new TimeOnly(14, 0)),
            Event("Zahnarzt", new DateOnly(2026, 3, 10), new TimeOnly(9, 0)),
            Event("Ferien", new DateOnly(2026, 3, 10), allDay: true),
            Event("Kino", new DateOnly(2026, 3, 11), new TimeOnly(20, 0)));
        await f.Context.SaveChangesAsync();

        var snapshot = await new WidgetDataService(f.Context, f.Time).GetDayAsync();

        Assert.Equal(["Ferien", "Zahnarzt", "Training"], snapshot.TodaysEvents.Select(e => e.Title));
    }

    [Fact]
    public async Task Day_next_event_is_the_earliest_from_today_onwards()
    {
        await using var f = new SqliteDbContextFixture();
        f.Context.AddRange(
            Event("Later", new DateOnly(2026, 3, 15), new TimeOnly(9, 0)),
            Event("Soon", new DateOnly(2026, 3, 12), new TimeOnly(9, 0)),
            Event("Past", new DateOnly(2026, 3, 5), new TimeOnly(9, 0)));
        await f.Context.SaveChangesAsync();

        var snapshot = await new WidgetDataService(f.Context, f.Time).GetDayAsync();

        Assert.Equal("Soon", snapshot.NextEvent!.Title);
    }

    [Fact]
    public async Task Day_study_plan_sums_minutes_and_excludes_past_or_zero_slices()
    {
        await using var f = new SqliteDbContextFixture();
        f.Context.AddRange(
            Exam("Mathematik", new DateOnly(2026, 3, 20), 30),
            Exam("Deutsch", new DateOnly(2026, 3, 25), 20),
            Exam("Franz", new DateOnly(2026, 3, 5), 40),   // past
            Exam("Bio", new DateOnly(2026, 3, 22), 0));     // no daily slice
        await f.Context.SaveChangesAsync();

        var snapshot = await new WidgetDataService(f.Context, f.Time).GetDayAsync();

        Assert.Equal(["Mathematik", "Deutsch"], snapshot.Study.Select(s => s.Subject));
        Assert.Equal(50, snapshot.TotalStudyMinutes);
    }

    [Fact]
    public async Task Day_on_an_empty_database_is_empty_but_not_null()
    {
        await using var f = new SqliteDbContextFixture();

        var snapshot = await new WidgetDataService(f.Context, f.Time).GetDayAsync();

        Assert.NotNull(snapshot);
        Assert.Empty(snapshot.TodaysEvents);
        Assert.Null(snapshot.NextEvent);
        Assert.Empty(snapshot.Study);
        Assert.Equal(0, snapshot.TotalStudyMinutes);
    }

    [Fact]
    public async Task Week_is_the_monday_first_week_containing_today()
    {
        await using var f = new SqliteDbContextFixture();
        f.Context.AddRange(
            Event("Monday", new DateOnly(2026, 3, 9)),
            Event("Sunday", new DateOnly(2026, 3, 15)),
            Event("PrevSunday", new DateOnly(2026, 3, 8)),
            Event("NextMonday", new DateOnly(2026, 3, 16)));
        await f.Context.SaveChangesAsync();

        var snapshot = await new WidgetDataService(f.Context, f.Time).GetWeekAsync();

        Assert.Equal(new DateOnly(2026, 3, 9), snapshot.WeekStart);
        Assert.Equal(["Monday", "Sunday"], snapshot.Events.Select(e => e.Title));
    }

    [Fact]
    public async Task Month_marks_the_current_month_days_that_have_events()
    {
        await using var f = new SqliteDbContextFixture();
        f.Context.AddRange(
            Event("a", new DateOnly(2026, 3, 3)),
            Event("b", new DateOnly(2026, 3, 12)),
            Event("c", new DateOnly(2026, 3, 12)),
            Event("d", new DateOnly(2026, 3, 19)),
            Event("prev", new DateOnly(2026, 2, 27))); // lands in the grid's lead, not a current-month day
        await f.Context.SaveChangesAsync();

        var snapshot = await new WidgetDataService(f.Context, f.Time).GetMonthAsync();

        Assert.Equal(42, snapshot.Days.Count);
        Assert.Equal([3, 12, 19], snapshot.DaysWithEvents);
        var today = Assert.Single(snapshot.Days, d => d.IsToday);
        Assert.True(today.IsCurrentMonth);
    }

    [Fact]
    public async Task Month_with_no_events_has_a_full_grid_and_no_marked_days()
    {
        await using var f = new SqliteDbContextFixture();

        var snapshot = await new WidgetDataService(f.Context, f.Time).GetMonthAsync();

        Assert.Equal(42, snapshot.Days.Count);
        Assert.Empty(snapshot.DaysWithEvents);
    }
}
