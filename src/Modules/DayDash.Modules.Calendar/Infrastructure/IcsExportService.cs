using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Infrastructure;

public class IcsExportService : IExportService
{
    public async Task ExportToIcsAsync(IEnumerable<CalendarEvent> events, string filePath, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        sb.AppendLine("BEGIN:VCALENDAR");
        sb.AppendLine("VERSION:2.0");
        sb.AppendLine("PRODID:-//DayDash//DayDash//DE");

        foreach (var calendarEvent in events)
        {
            var start = calendarEvent.Date.ToDateTime(calendarEvent.TimeFrom ?? TimeOnly.MinValue);
            var end = calendarEvent.Date.ToDateTime(calendarEvent.TimeTo ?? calendarEvent.TimeFrom ?? new TimeOnly(23, 59));

            sb.AppendLine("BEGIN:VEVENT");
            sb.AppendLine($"UID:{calendarEvent.Id}");
            sb.AppendLine(calendarEvent.IsAllDay
                ? $"DTSTART;VALUE=DATE:{calendarEvent.Date:yyyyMMdd}"
                : $"DTSTART:{start:yyyyMMddTHHmmss}");
            if (!calendarEvent.IsAllDay)
            {
                sb.AppendLine($"DTEND:{end:yyyyMMddTHHmmss}");
            }
            sb.AppendLine($"SUMMARY:{calendarEvent.Title}");
            sb.AppendLine($"DESCRIPTION:{calendarEvent.Notes}");
            sb.AppendLine("END:VEVENT");
        }

        sb.AppendLine("END:VCALENDAR");

        await File.WriteAllTextAsync(filePath, sb.ToString(), ct);
    }
}
