using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Application.Contracts;

public interface ICalendarService
{
    Task<IReadOnlyList<CalendarEvent>> GetEventsForMonthAsync(int year, int month, CancellationToken ct = default);
    Task<IReadOnlyList<CalendarEvent>> GetEventsInRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
    Task<IReadOnlyList<CalendarEvent>> GetEventsForWeekAsync(DateOnly startOfWeek, CancellationToken ct = default);
    Task<IReadOnlyList<CalendarEvent>> GetEventsForDayAsync(DateOnly date, CancellationToken ct = default);
    Task<CalendarEvent?> GetEventAsync(Guid eventId, CancellationToken ct = default);

    Task CreateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default);
    Task UpdateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default);
    Task DeleteEventAsync(Guid eventId, CancellationToken ct = default);

    Task<IReadOnlyList<EventTypeConfig>> GetEventTypesAsync(CancellationToken ct = default);
    Task SaveEventTypeAsync(EventTypeConfig eventTypeConfig, CancellationToken ct = default);
    Task DeleteEventTypeAsync(Guid eventTypeId, CancellationToken ct = default);

    /// <summary>Writes all events to <paramref name="filePath"/> as an iCalendar file.</summary>
    Task ExportIcsAsync(string filePath, CancellationToken ct = default);
}
