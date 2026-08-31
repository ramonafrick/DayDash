namespace DayDash.Modules.Calendar.Application.Models;

/// <summary>One row of a month grid: always seven consecutive days.</summary>
public sealed record CalendarWeek(IReadOnlyList<CalendarDayCell> Days);

/// <summary>One cell of a month grid.</summary>
public sealed record CalendarDayCell(
    DateOnly Date,
    bool IsCurrentMonth,
    bool IsToday,
    int EventCount,
    IReadOnlyList<string> DotColors);

/// <summary>Passed to the exam-assistant template when a "Prüfung" event is created (FR-C6).</summary>
public sealed record ExamAssistantRequest(string Title, DateOnly Date);
