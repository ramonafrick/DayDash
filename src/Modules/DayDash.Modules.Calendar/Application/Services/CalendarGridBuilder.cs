using DayDash.Modules.Calendar.Application.Models;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Application.Services;

/// <summary>
/// Pure builder for the month view's 6x7 grid. No I/O, no clock - <paramref name="today"/> and
/// the event list are supplied by the caller so it is fully deterministic and unit-testable.
/// </summary>
public static class CalendarGridBuilder
{
    public const int WeekCount = 6;
    public const int DaysPerWeek = 7;

    public static IReadOnlyList<CalendarWeek> Build(
        int year,
        int month,
        DayOfWeek firstDayOfWeek,
        DateOnly today,
        IReadOnlyList<CalendarEvent> events,
        IReadOnlyList<EventTypeConfig> eventTypes)
    {
        var firstOfMonth = new DateOnly(year, month, 1);

        // Number of leading days from the previous month so row 1 starts on firstDayOfWeek.
        var lead = ((int)firstOfMonth.DayOfWeek - (int)firstDayOfWeek + DaysPerWeek) % DaysPerWeek;
        var gridStart = firstOfMonth.AddDays(-lead);

        var colorByTypeId = eventTypes.ToDictionary(t => t.Id, t => t.Color);

        var byDay = events
            .GroupBy(e => e.Date)
            .ToDictionary(g => g.Key, g => g.ToList());

        var weeks = new List<CalendarWeek>(WeekCount);
        for (var w = 0; w < WeekCount; w++)
        {
            var cells = new List<CalendarDayCell>(DaysPerWeek);
            for (var d = 0; d < DaysPerWeek; d++)
            {
                var date = gridStart.AddDays(w * DaysPerWeek + d);
                byDay.TryGetValue(date, out var dayEvents);

                var colors = dayEvents is null
                    ? []
                    : dayEvents
                        .Select(e => ResolveColor(e, colorByTypeId))
                        .Where(c => c is not null)
                        .Select(c => c!)
                        .Distinct()
                        .ToList();

                cells.Add(new CalendarDayCell(
                    Date: date,
                    IsCurrentMonth: date.Month == month && date.Year == year,
                    IsToday: date == today,
                    EventCount: dayEvents?.Count ?? 0,
                    DotColors: colors));
            }

            weeks.Add(new CalendarWeek(cells));
        }

        return weeks;
    }

    private static string? ResolveColor(CalendarEvent e, IReadOnlyDictionary<Guid, string> colorByTypeId)
    {
        if (e.EventType is not null)
        {
            return e.EventType.Color;
        }

        return e.EventTypeId is { } id && colorByTypeId.TryGetValue(id, out var color) ? color : null;
    }
}
