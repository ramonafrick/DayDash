using DayDash.Modules.Calendar.Application.Services;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Calendar;

public class IcsBuilderTests
{
    private static string Unfold(string ics) => ics.Replace("\r\n ", string.Empty);

    [Fact]
    public void Wraps_events_in_a_valid_VCALENDAR_frame()
    {
        var ics = IcsBuilder.Build([TestData.AnEvent()]);

        Assert.StartsWith("BEGIN:VCALENDAR\r\n", ics);
        Assert.Contains("VERSION:2.0\r\n", ics);
        Assert.EndsWith("END:VCALENDAR\r\n", ics);
        Assert.Contains("BEGIN:VEVENT\r\n", ics);
        Assert.Contains("END:VEVENT\r\n", ics);
    }

    [Fact]
    public void Empty_event_list_still_produces_a_valid_empty_calendar()
    {
        var ics = IcsBuilder.Build([]);

        Assert.StartsWith("BEGIN:VCALENDAR\r\n", ics);
        Assert.EndsWith("END:VCALENDAR\r\n", ics);
        Assert.DoesNotContain("VEVENT", ics);
    }

    [Fact]
    public void All_day_event_uses_VALUE_DATE_and_no_time_component()
    {
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 10), allDay: true);

        var ics = IcsBuilder.Build([e]);

        Assert.Contains("DTSTART;VALUE=DATE:20260310\r\n", ics);
        Assert.Contains("DTEND;VALUE=DATE:20260311\r\n", ics);
        Assert.DoesNotContain("DTSTART:2026", ics);
    }

    [Fact]
    public void Timed_event_without_TimeTo_falls_back_to_end_of_day()
    {
        var e = TestData.AnEvent(date: new DateOnly(2026, 3, 10), from: new TimeOnly(9, 0), to: null);

        var ics = IcsBuilder.Build([e]);

        Assert.Contains("DTSTART:20260310T090000\r\n", ics);
        Assert.Contains("DTEND:20260310T235959\r\n", ics);
    }

    [Fact]
    public void Special_characters_in_summary_and_notes_are_escaped()
    {
        var e = TestData.AnEvent(title: "Math; test, part\\1");
        e.Notes = "line one\nline two";

        var ics = Unfold(IcsBuilder.Build([e]));

        Assert.Contains("SUMMARY:Math\\; test\\, part\\\\1\r\n", ics);
        Assert.Contains("DESCRIPTION:line one\\nline two\r\n", ics);
    }

    [Fact]
    public void Null_notes_emits_no_DESCRIPTION_line()
    {
        var e = TestData.AnEvent();
        e.Notes = null;

        var ics = IcsBuilder.Build([e]);

        Assert.DoesNotContain("DESCRIPTION:", ics);
    }
}
