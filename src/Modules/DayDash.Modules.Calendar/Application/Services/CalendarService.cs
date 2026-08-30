using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DayDash.Modules.Calendar.Application.Services;

public class CalendarService(ICalendarRepository repository, ILogger<CalendarService> logger) : ICalendarService
{
    public async Task<IReadOnlyList<CalendarEvent>> GetEventsForMonthAsync(int year, int month, CancellationToken ct = default)
        => await repository.GetByMonthAsync(year, month, ct);

    public async Task<IReadOnlyList<CalendarEvent>> GetEventsForWeekAsync(DateOnly startOfWeek, CancellationToken ct = default)
    {
        var endOfWeek = startOfWeek.AddDays(6);
        return await repository.GetByDateRangeAsync(startOfWeek, endOfWeek, ct);
    }

    public async Task<IReadOnlyList<CalendarEvent>> GetEventsForDayAsync(DateOnly date, CancellationToken ct = default)
        => await repository.GetByDateAsync(date, ct);

    public async Task CreateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default)
        => await repository.AddAsync(calendarEvent, ct);

    public async Task UpdateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default)
        => await repository.UpdateAsync(calendarEvent, ct);

    public async Task DeleteEventAsync(Guid eventId, CancellationToken ct = default)
    {
        var calendarEvent = await repository.GetByIdAsync(eventId, ct);
        if (calendarEvent != null)
        {
            await repository.DeleteAsync(calendarEvent, ct);
        }
    }

    public async Task<IReadOnlyList<EventTypeConfig>> GetEventTypesAsync(CancellationToken ct = default)
        => await repository.Context.Set<EventTypeConfig>().ToListAsync(ct);

    public async Task CreateEventTypeAsync(EventTypeConfig eventTypeConfig, CancellationToken ct = default)
    {
        await repository.Context.Set<EventTypeConfig>().AddAsync(eventTypeConfig, ct);
        await repository.Context.SaveChangesAsync(ct);
    }
}