using System.Text;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Application.Services;

/// <summary>
/// Pure iCalendar (RFC 5545) serializer for a set of <see cref="CalendarEvent"/>s. No I/O -
/// <see cref="Infrastructure.IcsExportService"/> writes the result to a file.
/// </summary>
public static class IcsBuilder
{
    private const string Crlf = "\r\n";

    public static string Build(IEnumerable<CalendarEvent> events)
    {
        var sb = new StringBuilder();
        AppendLine(sb, "BEGIN:VCALENDAR");
        AppendLine(sb, "VERSION:2.0");
        AppendLine(sb, "PRODID:-//DayDash//DayDash//EN");
        AppendLine(sb, "CALSCALE:GREGORIAN");

        foreach (var e in events)
        {
            AppendLine(sb, "BEGIN:VEVENT");
            AppendLine(sb, $"UID:{e.Id:D}@daydash");

            if (e.IsAllDay)
            {
                AppendLine(sb, $"DTSTART;VALUE=DATE:{e.Date:yyyyMMdd}");
                AppendLine(sb, $"DTEND;VALUE=DATE:{e.Date.AddDays(1):yyyyMMdd}");
            }
            else
            {
                var start = e.Date.ToDateTime(e.TimeFrom ?? TimeOnly.MinValue);
                // No explicit end -> run to the end of the day rather than emit a zero-length event.
                var end = e.Date.ToDateTime(e.TimeTo ?? new TimeOnly(23, 59, 59));
                AppendLine(sb, $"DTSTART:{start:yyyyMMddTHHmmss}");
                AppendLine(sb, $"DTEND:{end:yyyyMMddTHHmmss}");
            }

            AppendLine(sb, $"SUMMARY:{Escape(e.Title)}");
            if (!string.IsNullOrEmpty(e.Notes))
            {
                AppendLine(sb, $"DESCRIPTION:{Escape(e.Notes)}");
            }

            AppendLine(sb, "END:VEVENT");
        }

        AppendLine(sb, "END:VCALENDAR");
        return sb.ToString();
    }

    private static string Escape(string value) => value
        .Replace("\\", "\\\\")
        .Replace(";", "\\;")
        .Replace(",", "\\,")
        .Replace("\r\n", "\\n")
        .Replace("\n", "\\n")
        .Replace("\r", "\\n");

    /// <summary>Appends a content line, folding at 75 octets per RFC 5545 §3.1.</summary>
    private static void AppendLine(StringBuilder sb, string line)
    {
        const int max = 75;
        if (line.Length <= max)
        {
            sb.Append(line).Append(Crlf);
            return;
        }

        sb.Append(line[..max]).Append(Crlf);
        var rest = line[max..];
        while (rest.Length > max - 1)
        {
            sb.Append(' ').Append(rest[..(max - 1)]).Append(Crlf);
            rest = rest[(max - 1)..];
        }

        sb.Append(' ').Append(rest).Append(Crlf);
    }
}
