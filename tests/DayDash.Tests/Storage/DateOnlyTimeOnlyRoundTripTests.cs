using DayDash.Modules.Calendar.Domain;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.Storage;

public class DateOnlyTimeOnlyRoundTripTests
{
    [Fact]
    public async Task DateOnly_and_TimeOnly_survive_save_detach_reload()
    {
        await using var f = new SqliteDbContextFixture();
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 10), from: new TimeOnly(15, 30), to: new TimeOnly(16, 45));
        f.Context.Add(e);
        await f.Context.SaveChangesAsync();
        f.Context.ChangeTracker.Clear();

        var loaded = await f.Context.Set<CalendarEvent>().SingleAsync(x => x.Id == e.Id);

        Assert.Equal(new DateOnly(2026, 3, 10), loaded.Date);
        Assert.Equal(new TimeOnly(15, 30), loaded.TimeFrom);
        Assert.Equal(new TimeOnly(16, 45), loaded.TimeTo);
    }

    [Fact]
    public async Task Ordering_by_TimeFrom_is_chronological()
    {
        await using var f = new SqliteDbContextFixture();
        var day = new DateOnly(2026, 3, 10);
        f.Context.Add(TestData.AnEvent(title: "afternoon", date: day, from: new TimeOnly(14, 0)));
        f.Context.Add(TestData.AnEvent(title: "morning", date: day, from: new TimeOnly(8, 0)));
        f.Context.Add(TestData.AnEvent(title: "evening", date: day, from: new TimeOnly(19, 30)));
        await f.Context.SaveChangesAsync();

        var ordered = await f.Context.Set<CalendarEvent>().OrderBy(e => e.TimeFrom).Select(e => e.Title).ToListAsync();

        Assert.Equal(new[] { "morning", "afternoon", "evening" }, ordered);
    }
}
