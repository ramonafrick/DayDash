using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Calendar.Application.Services;

public class CalendarService(
    ICalendarRepository repository,
    IExportService exportService,
    IDataChangeNotifier notifier) : ICalendarService
{
    public Task<IReadOnlyList<CalendarEvent>> GetEventsForMonthAsync(int year, int month, CancellationToken ct = default)
        => repository.GetByMonthAsync(year, month, ct);

    public Task<IReadOnlyList<CalendarEvent>> GetEventsInRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default)
        => repository.GetByDateRangeAsync(from, to, ct);

    public Task<IReadOnlyList<CalendarEvent>> GetEventsForWeekAsync(DateOnly startOfWeek, CancellationToken ct = default)
        => repository.GetByDateRangeAsync(startOfWeek, startOfWeek.AddDays(6), ct);

    public Task<IReadOnlyList<CalendarEvent>> GetEventsForDayAsync(DateOnly date, CancellationToken ct = default)
        => repository.GetByDateAsync(date, ct);

    public Task<CalendarEvent?> GetEventAsync(Guid eventId, CancellationToken ct = default)
        => repository.GetByIdAsync(eventId, ct);

    public async Task CreateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default)
    {
        await repository.AddAsync(calendarEvent, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.CalendarEventSaved, calendarEvent.Id), ct);
    }

    public async Task UpdateEventAsync(CalendarEvent calendarEvent, CancellationToken ct = default)
    {
        await repository.UpdateAsync(calendarEvent, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.CalendarEventSaved, calendarEvent.Id), ct);
    }

    public async Task DeleteEventAsync(Guid eventId, CancellationToken ct = default)
    {
        var calendarEvent = await repository.GetByIdAsync(eventId, ct);
        if (calendarEvent is null)
        {
            return;
        }

        await repository.DeleteAsync(calendarEvent, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.CalendarEventDeleted, eventId), ct);
    }

    public async Task<IReadOnlyList<EventTypeConfig>> GetEventTypesAsync(CancellationToken ct = default)
        => await repository.Context.Set<EventTypeConfig>().AsNoTracking().OrderBy(t => t.Name).ToListAsync(ct);

    public async Task SaveEventTypeAsync(EventTypeConfig eventTypeConfig, CancellationToken ct = default)
    {
        var set = repository.Context.Set<EventTypeConfig>();
        var existing = await set.FindAsync([eventTypeConfig.Id], ct);
        if (existing is null)
        {
            set.Add(eventTypeConfig);
        }
        else
        {
            existing.Name = eventTypeConfig.Name;
            existing.Color = eventTypeConfig.Color;
            // Key and IsDefault are never changed by a rename.
        }

        await repository.Context.SaveChangesAsync(ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.EventTypeChanged, eventTypeConfig.Id), ct);
    }

    public async Task DeleteEventTypeAsync(Guid eventTypeId, CancellationToken ct = default)
    {
        var set = repository.Context.Set<EventTypeConfig>();
        var existing = await set.FindAsync([eventTypeId], ct);
        if (existing is null || existing.IsDefault)
        {
            return; // built-in types cannot be deleted (their Key drives the exam-assistant trigger).
        }

        set.Remove(existing); // FK is DeleteBehavior.SetNull - events keep their row, EventTypeId is cleared.
        await repository.Context.SaveChangesAsync(ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.EventTypeChanged, eventTypeId), ct);
    }

    public async Task ExportIcsAsync(string filePath, CancellationToken ct = default)
    {
        var all = await repository.GetAllAsync(ct);
        await exportService.ExportToIcsAsync(all, filePath, ct);
    }
}
