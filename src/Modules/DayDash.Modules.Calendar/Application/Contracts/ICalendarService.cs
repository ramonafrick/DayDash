using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Application.Contracts;

public interface ICalendarService
{
    Task<IReadOnlyList<CalendarEvent>> GetEventsForMonthAsync(int year, int month, CancellationToken ct = default);
    Task<IReadOnlyList<CalendarEvent>> GetEventsForWeekAsync(DateOnly startOfWeek, CancellationToken ct = default);
    Task<IReadOnlyList<CalendarEvent>> GetEventsForDayAsync(DateOnly date, CancellationToken ct = default);

    Task CreateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default);
    Task UpdateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default);
    Task DeleteEventAsync(Guid eventId, CancellationToken ct = default);

    Task<IReadOnlyList<EventTypeConfig>> GetEventTypesAsync(CancellationToken ct = default);
    Task CreateEventTypeAsync(EventTypeConfig eventTypeConfig, CancellationToken ct = default);
}