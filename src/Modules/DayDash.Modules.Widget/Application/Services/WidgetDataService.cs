using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.Widget.Application.Contracts;
using DayDash.Modules.Widget.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Widget.Application.Services;

/// <summary>
/// Read-only projections for the widgets. Bounded <see cref="EntityFrameworkQueryableExtensions.AsNoTracking{TSource}"/>
/// queries only - never writes, never migrates.
/// </summary>
public sealed class WidgetDataService(DayDashDbContext context, TimeProvider timeProvider) : IWidgetDataService
{
    private DateOnly Today => DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);

    public async Task<WidgetDaySnapshot> GetDayAsync(CancellationToken ct = default)
    {
        var today = Today;

        var todaysEvents = await context.Set<CalendarEvent>().AsNoTracking()
            .Where(e => e.Date == today)
            .OrderByDescending(e => e.IsAllDay).ThenBy(e => e.TimeFrom).ThenBy(e => e.Title)
            .Select(e => new WidgetEventItem(e.Title, e.Date, e.TimeFrom, e.IsAllDay))
            .ToListAsync(ct);

        var nextEvent = await context.Set<CalendarEvent>().AsNoTracking()
            .Where(e => e.Date >= today)
            .OrderBy(e => e.Date).ThenByDescending(e => e.IsAllDay).ThenBy(e => e.TimeFrom)
            .Select(e => new WidgetEventItem(e.Title, e.Date, e.TimeFrom, e.IsAllDay))
            .FirstOrDefaultAsync(ct);

        var study = await context.Set<Exam>().AsNoTracking()
            .Where(x => x.DailyMinutes > 0 && x.ExamDate >= today)
            .OrderBy(x => x.ExamDate)
            .Select(x => new WidgetStudyItem(x.Subject, x.DailyMinutes))
            .ToListAsync(ct);

        return new WidgetDaySnapshot(today, todaysEvents, nextEvent, study, study.Sum(s => s.Minutes));
    }

    public async Task<WidgetWeekSnapshot> GetWeekAsync(CancellationToken ct = default)
    {
        var weekStart = StartOfWeek(Today);
        var weekEnd = weekStart.AddDays(7);

        var events = await context.Set<CalendarEvent>().AsNoTracking()
            .Where(e => e.Date >= weekStart && e.Date < weekEnd)
            .OrderBy(e => e.Date).ThenByDescending(e => e.IsAllDay).ThenBy(e => e.TimeFrom)
            .Select(e => new WidgetEventItem(e.Title, e.Date, e.TimeFrom, e.IsAllDay))
            .ToListAsync(ct);

        return new WidgetWeekSnapshot(weekStart, events);
    }

    public async Task<WidgetMonthSnapshot> GetMonthAsync(CancellationToken ct = default)
    {
        var today = Today;
        var firstOfMonth = new DateOnly(today.Year, today.Month, 1);
        var lead = ((int)firstOfMonth.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
        var gridStart = firstOfMonth.AddDays(-lead);
        var gridEnd = gridStart.AddDays(CalendarGridBuilder.WeekCount * CalendarGridBuilder.DaysPerWeek);

        var events = await context.Set<CalendarEvent>().AsNoTracking()
            .Where(e => e.Date >= gridStart && e.Date < gridEnd)
            .ToListAsync(ct);

        var grid = CalendarGridBuilder.Build(today.Year, today.Month, DayOfWeek.Monday, today, events, []);
        var cells = grid.SelectMany(w => w.Days).ToList();

        var days = cells
            .Select(c => new WidgetMonthDay(c.Date, c.IsCurrentMonth, c.IsToday, c.EventCount > 0))
            .ToList();

        var daysWithEvents = cells
            .Where(c => c.IsCurrentMonth && c.EventCount > 0)
            .Select(c => c.Date.Day)
            .Distinct()
            .OrderBy(d => d)
            .ToList();

        return new WidgetMonthSnapshot(today.Year, today.Month, days, daysWithEvents);
    }

    private static DateOnly StartOfWeek(DateOnly date)
        => date.AddDays(-(((int)date.DayOfWeek + 6) % 7)); // Monday-first
}
