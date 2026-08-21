using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Domain;

namespace DayDash.Modules.Storage.Infrastructure
{
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
                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine($"UID:{Guid.NewGuid()}");
                sb.AppendLine($"DTSTART:{calendarEvent.StartDate:yyyyMMddTHHmmssZ}");
                sb.AppendLine($"DTEND:{calendarEvent.EndDate:yyyyMMddTHHmmssZ}");
                sb.AppendLine($"SUMMARY:{calendarEvent.Title}");
                sb.AppendLine($"DESCRIPTION:{calendarEvent.Description}");
                sb.AppendLine("END:VEVENT");
            }

            sb.AppendLine("END:VCALENDAR");

            await File.WriteAllTextAsync(filePath, sb.ToString(), ct);
        }
    }
}