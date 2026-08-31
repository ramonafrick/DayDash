using System.Globalization;
using System.Text;
using DayDash.Modules.Widget.Application.Models;
using DayDash.Modules.Widget.Resources;

namespace DayDash.Modules.Widget.Application.Services;

/// <summary>
/// Turns a widget snapshot into the localized text the RemoteViews show. Pure - reads
/// <see cref="WidgetResources"/> under its current <see cref="WidgetResources.Culture"/>.
/// A <c>null</c> snapshot (the data layer failed) renders the "no data" string.
/// </summary>
public static class WidgetTextFormatter
{
    public static string DayEvents(WidgetDaySnapshot? snapshot)
    {
        if (snapshot is null)
        {
            return WidgetResources.DataUnavailable;
        }

        if (snapshot.TodaysEvents.Count > 0)
        {
            return string.Join("\n", snapshot.TodaysEvents.Select(Describe));
        }

        return snapshot.NextEvent is { } next
            ? $"{WidgetResources.NextEvent}: {Describe(next)} ({FormatDate(next.Date)})"
            : WidgetResources.NoEventsToday;
    }

    public static string DayStudy(WidgetDaySnapshot? snapshot)
    {
        if (snapshot is null)
        {
            return WidgetResources.DataUnavailable;
        }

        if (snapshot.Study.Count == 0)
        {
            return WidgetResources.NoStudyToday;
        }

        var header = $"{WidgetResources.StudyPlan} · {snapshot.TotalStudyMinutes} {WidgetResources.Minutes}";
        var lines = snapshot.Study.Select(s => $"{s.Subject} · {s.Minutes} {WidgetResources.Minutes}");
        return header + "\n" + string.Join("\n", lines);
    }

    public static string WeekEvents(WidgetWeekSnapshot? snapshot)
    {
        if (snapshot is null)
        {
            return WidgetResources.DataUnavailable;
        }

        if (snapshot.Events.Count == 0)
        {
            return WidgetResources.NoEventsThisWeek;
        }

        return string.Join("\n", snapshot.Events
            .GroupBy(e => e.Date)
            .Select(g => $"{FormatWeekday(g.Key)} {g.Key.Day:D2}. · " + string.Join(", ", g.Select(e => e.Title))));
    }

    public static string MonthGrid(WidgetMonthSnapshot? snapshot)
    {
        if (snapshot is null || snapshot.Days.Count == 0)
        {
            return WidgetResources.DataUnavailable;
        }

        // Fixed 3-char cells so the monospaced header lines up with the columns.
        var sb = new StringBuilder();
        sb.AppendLine(string.Join(" ", WidgetResources.WeekdayHeadings.Select(h => (h.Length >= 2 ? h[..2] : h).PadRight(3))));

        for (var week = 0; week < 6; week++)
        {
            var cells = snapshot.Days.Skip(week * 7).Take(7).Select(d =>
            {
                if (!d.IsCurrentMonth)
                {
                    return "   ";
                }

                var marker = (d.IsToday, d.HasEvents) switch
                {
                    (true, true) => "+",
                    (true, false) => ".",
                    (false, true) => "*",
                    _ => " ",
                };
                return $"{d.Date.Day,-2}{marker}";
            });
            sb.AppendLine(string.Join(" ", cells));
        }

        return sb.ToString().TrimEnd();
    }

    private static string Describe(WidgetEventItem e)
        => e.IsAllDay || e.Time is null
            ? $"{WidgetResources.AllDay} · {e.Title}"
            : $"{e.Time.Value.ToString("HH\\:mm", CultureInfo.CurrentCulture)} · {e.Title}";

    private static string FormatDate(DateOnly date)
        => date.ToString("d", WidgetResources.Culture ?? CultureInfo.CurrentCulture);

    private static string FormatWeekday(DateOnly date) => date.DayOfWeek switch
    {
        DayOfWeek.Monday => WidgetResources.Weekday_Mon,
        DayOfWeek.Tuesday => WidgetResources.Weekday_Tue,
        DayOfWeek.Wednesday => WidgetResources.Weekday_Wed,
        DayOfWeek.Thursday => WidgetResources.Weekday_Thu,
        DayOfWeek.Friday => WidgetResources.Weekday_Fri,
        DayOfWeek.Saturday => WidgetResources.Weekday_Sat,
        _ => WidgetResources.Weekday_Sun,
    };
}
