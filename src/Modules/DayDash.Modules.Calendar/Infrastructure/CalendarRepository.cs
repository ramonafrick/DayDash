using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Calendar.Infrastructure;

public class CalendarRepository(DayDashDbContext context) : BaseRepository<CalendarEvent>(context), ICalendarRepository
{
    public async Task<IReadOnlyList<CalendarEvent>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => await OrderedQuery().Where(e => e.Date == date).ToListAsync(ct);

    public async Task<IReadOnlyList<CalendarEvent>> GetByMonthAsync(int year, int month, CancellationToken ct = default)
    {
        if (month is < 1 or > 12)
        {
            return [];
        }

        // Half-open range instead of e.Date.Year/.Month so it translates on any provider (AD-2).
        var first = new DateOnly(year, month, 1);
        var next = first.AddMonths(1);
        return await OrderedQuery().Where(e => e.Date >= first && e.Date < next).ToListAsync(ct);
    }

    /// <summary>Inclusive on both ends, by contract.</summary>
    public async Task<IReadOnlyList<CalendarEvent>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
        => await OrderedQuery().Where(e => e.Date >= from && e.Date <= to).ToListAsync(ct);

    private IQueryable<CalendarEvent> OrderedQuery() =>
        _context.Set<CalendarEvent>()
            .AsNoTracking()
            .Include(e => e.EventType)
            .OrderBy(e => e.Date)
            .ThenBy(e => e.TimeFrom);
}
