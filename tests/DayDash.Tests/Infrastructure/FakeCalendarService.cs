using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Tests.Infrastructure;

/// <summary>In-memory <see cref="ICalendarService"/> for component tests.</summary>
public sealed class FakeCalendarService : ICalendarService
{
    public List<CalendarEvent> Events { get; } = [];
    public List<EventTypeConfig> Types { get; } = [];

    public int MonthQueries { get; private set; }
    public int RangeQueries { get; private set; }
    public int WeekQueries { get; private set; }
    public (int Year, int Month) LastMonthQuery { get; private set; }
    public (DateOnly From, DateOnly To) LastRangeQuery { get; private set; }
    public CalendarEvent? LastCreated { get; private set; }
    public CalendarEvent? LastUpdated { get; private set; }
    public Guid? LastDeleted { get; private set; }
    public string? LastExportPath { get; private set; }

    public Task<IReadOnlyList<CalendarEvent>> GetEventsForMonthAsync(int year, int month, CancellationToken ct = default)
    {
        MonthQueries++;
        LastMonthQuery = (year, month);
        return Task.FromResult<IReadOnlyList<CalendarEvent>>(
            Events.Where(e => e.Date.Year == year && e.Date.Month == month).ToList());
    }

    public Task<IReadOnlyList<CalendarEvent>> GetEventsInRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
    {
        RangeQueries++;
        LastRangeQuery = (from, to);
        return Task.FromResult<IReadOnlyList<CalendarEvent>>(
            Events.Where(e => e.Date >= from && e.Date <= to).ToList());
    }

    public Task<IReadOnlyList<CalendarEvent>> GetEventsForWeekAsync(DateOnly startOfWeek, CancellationToken ct = default)
    {
        WeekQueries++;
        var end = startOfWeek.AddDays(6);
        return Task.FromResult<IReadOnlyList<CalendarEvent>>(
            Events.Where(e => e.Date >= startOfWeek && e.Date <= end).ToList());
    }

    public Task<IReadOnlyList<CalendarEvent>> GetEventsForDayAsync(DateOnly date, CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<CalendarEvent>>(Events.Where(e => e.Date == date).ToList());

    public Task<CalendarEvent?> GetEventAsync(Guid eventId, CancellationToken ct = default)
        => Task.FromResult(Events.FirstOrDefault(e => e.Id == eventId));

    public Task CreateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default)
    {
        LastCreated = calendarEvent;
        Events.Add(calendarEvent);
        return Task.CompletedTask;
    }

    public Task UpdateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default)
    {
        LastUpdated = calendarEvent;
        return Task.CompletedTask;
    }

    public Task DeleteEventAsync(Guid eventId, CancellationToken ct = default)
    {
        LastDeleted = eventId;
        Events.RemoveAll(e => e.Id == eventId);
        return Task.CompletedTask;
    }

    public Task<IReadOnlyList<EventTypeConfig>> GetEventTypesAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<EventTypeConfig>>(Types.ToList());

    public Task SaveEventTypeAsync(EventTypeConfig eventTypeConfig, CancellationToken ct = default)
    {
        Types.RemoveAll(t => t.Id == eventTypeConfig.Id);
        Types.Add(eventTypeConfig);
        return Task.CompletedTask;
    }

    public Task DeleteEventTypeAsync(Guid eventTypeId, CancellationToken ct = default)
    {
        Types.RemoveAll(t => t.Id == eventTypeId);
        return Task.CompletedTask;
    }

    public Task ExportIcsAsync(string filePath, CancellationToken ct = default)
    {
        LastExportPath = filePath;
        return Task.CompletedTask;
    }
}

public sealed class FakeFileShareService : DayDash.Modules.Settings.Application.Contracts.IFileShareService
{
    public string? LastPath { get; private set; }

    public Task ShareFileAsync(string filePath, string title, CancellationToken ct = default)
    {
        LastPath = filePath;
        return Task.CompletedTask;
    }
}
