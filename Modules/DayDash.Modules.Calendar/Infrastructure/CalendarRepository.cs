using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Calendar.Infrastructure;

public class CalendarRepository(DayDashDbContext context) : BaseRepository<CalendarEvent>(context), ICalendarRepository
{
    public async Task<IReadOnlyList<CalendarEvent>> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        return await _context.CalendarEvents
            .Where(e => e.Date == date)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetByMonthAsync(int year, int month, CancellationToken ct = default)
    {
        return await _context.CalendarEvents
            .Where(e => e.Date.Year == year && e.Date.Month == month)
            .ToListAsync(ct);
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        return await _context.CalendarEvents
            .Where(e => e.Date >= from && e.Date <= to)
            .ToListAsync(ct);
    }
}