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
    private const int MaxOctets = 75;

    public static string Build(IEnumerable<CalendarEvent> events, DateTimeOffset generatedAt)
    {
        var stamp = generatedAt.UtcDateTime.ToString("yyyyMMddTHHmmssZ");

        var sb = new StringBuilder();
        AppendLine(sb, "BEGIN:VCALENDAR");
        AppendLine(sb, "VERSION:2.0");
        AppendLine(sb, "PRODID:-//DayDash//DayDash//EN");
        AppendLine(sb, "CALSCALE:GREGORIAN");

        foreach (var e in events)
        {
            AppendLine(sb, "BEGIN:VEVENT");
            AppendLine(sb, $"UID:{e.Id:D}@daydash");
            AppendLine(sb, $"DTSTAMP:{stamp}");

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

    /// <summary>
    /// Appends a content line, folding at 75 <em>octets</em> (UTF-8) per RFC 5545 §3.1 without
    /// splitting a multi-byte character across the fold.
    /// </summary>
    private static void AppendLine(StringBuilder sb, string line)
    {
        var first = true;
        var index = 0;
        while (index < line.Length)
        {
            // A continuation line starts with a single space that also counts toward the limit.
            var budget = first ? MaxOctets : MaxOctets - 1;
            var take = 0;
            var octets = 0;
            while (index + take < line.Length)
            {
                var runeLen = char.IsHighSurrogate(line[index + take]) ? 2 : 1;
                var rune = char.ConvertToUtf32(line.Substring(index + take, runeLen), 0);
                var runeOctets = Utf8Length(rune);
                if (octets + runeOctets > budget)
                {
                    break;
                }

                octets += runeOctets;
                take += runeLen;
            }

            if (take == 0)
            {
                take = Math.Min(line.Length - index, 1); // guard against a rune wider than the budget
            }

            if (!first)
            {
                sb.Append(' ');
            }

            sb.Append(line, index, take).Append(Crlf);
            index += take;
            first = false;
        }

        if (first)
        {
            sb.Append(Crlf); // empty line
        }
    }

    private static int Utf8Length(int codePoint) => codePoint switch
    {
        <= 0x7F => 1,
        <= 0x7FF => 2,
        <= 0xFFFF => 3,
        _ => 4,
    };
}
