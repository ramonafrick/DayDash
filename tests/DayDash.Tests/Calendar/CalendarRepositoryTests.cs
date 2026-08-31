using DayDash.Modules.Calendar.Infrastructure;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Calendar;

public class CalendarRepositoryTests
{
    private static async Task<CalendarRepository> SeededAsync(SqliteDbContextFixture f, params DateOnly[] dates)
    {
        foreach (var d in dates)
        {
            f.Context.Add(TestData.AnEvent(date: d));
        }

        await f.Context.SaveChangesAsync();
        return new CalendarRepository(f.Context);
    }

    [Fact]
    public async Task GetByMonth_includes_the_first_and_last_day_and_excludes_neighbours()
    {
        await using var f = new SqliteDbContextFixture();
        var repo = await SeededAsync(f,
            new DateOnly(2026, 2, 28),  // previous month, excluded
            new DateOnly(2026, 3, 1),   // included
            new DateOnly(2026, 3, 31),  // included
            new DateOnly(2026, 4, 1));  // next month, excluded

        var march = await repo.GetByMonthAsync(2026, 3);

        Assert.Equal(2, march.Count);
        Assert.All(march, e => Assert.Equal(3, e.Date.Month));
    }

    [Fact]
    public async Task GetByMonth_handles_the_December_to_January_boundary()
    {
        await using var f = new SqliteDbContextFixture();
        var repo = await SeededAsync(f,
            new DateOnly(2026, 12, 31),
            new DateOnly(2027, 1, 1));

        var december = await repo.GetByMonthAsync(2026, 12);

        Assert.Single(december);
        Assert.Equal(new DateOnly(2026, 12, 31), december[0].Date);
    }

    [Fact]
    public async Task GetByMonth_includes_the_leap_day()
    {
        await using var f = new SqliteDbContextFixture();
        var repo = await SeededAsync(f, new DateOnly(2028, 2, 29));

        Assert.Single(await repo.GetByMonthAsync(2028, 2));
    }

    [Fact]
    public async Task GetByMonth_returns_empty_for_a_month_with_no_events()
    {
        await using var f = new SqliteDbContextFixture();
        var repo = await SeededAsync(f, new DateOnly(2026, 3, 10));

        Assert.Empty(await repo.GetByMonthAsync(2026, 4));
    }

    [Fact]
    public async Task GetByDateRange_is_inclusive_on_both_ends_and_ordered()
    {
        await using var f = new SqliteDbContextFixture();
        var repo = await SeededAsync(f,
            new DateOnly(2026, 3, 12),
            new DateOnly(2026, 3, 10),
            new DateOnly(2026, 3, 15),  // upper bound, included
            new DateOnly(2026, 3, 9));  // below lower bound, excluded

        var range = await repo.GetByDateRangeAsync(new DateOnly(2026, 3, 10), new DateOnly(2026, 3, 15));

        Assert.Equal(
            new[] { new DateOnly(2026, 3, 10), new DateOnly(2026, 3, 12), new DateOnly(2026, 3, 15) },
            range.Select(e => e.Date));
    }
}
