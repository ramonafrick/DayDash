namespace DayDash.Modules.Widget.Application.Models;

/// <summary>One calendar event as shown on a widget.</summary>
public sealed record WidgetEventItem(string Title, DateOnly Date, TimeOnly? Time, bool IsAllDay);

/// <summary>One exam's study slice for today.</summary>
public sealed record WidgetStudyItem(string Subject, int Minutes);

/// <summary>What the day widget shows.</summary>
public sealed record WidgetDaySnapshot(
    DateOnly Date,
    IReadOnlyList<WidgetEventItem> TodaysEvents,
    WidgetEventItem? NextEvent,
    IReadOnlyList<WidgetStudyItem> Study,
    int TotalStudyMinutes)
{
    public static WidgetDaySnapshot Empty(DateOnly date) => new(date, [], null, [], 0);
}

/// <summary>What the week widget shows: this week's events, Monday-first.</summary>
public sealed record WidgetWeekSnapshot(DateOnly WeekStart, IReadOnlyList<WidgetEventItem> Events)
{
    public static WidgetWeekSnapshot Empty(DateOnly weekStart) => new(weekStart, []);
}

/// <summary>One cell of the month widget's mini grid.</summary>
public sealed record WidgetMonthDay(DateOnly Date, bool IsCurrentMonth, bool IsToday, bool HasEvents);

/// <summary>What the month widget shows: a 6x7 grid plus the current-month days that have events.</summary>
public sealed record WidgetMonthSnapshot(
    int Year,
    int Month,
    IReadOnlyList<WidgetMonthDay> Days,
    IReadOnlyList<int> DaysWithEvents)
{
    public static WidgetMonthSnapshot Empty(int year, int month) => new(year, month, [], []);
}
