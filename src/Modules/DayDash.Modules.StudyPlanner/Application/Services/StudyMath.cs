namespace DayDash.Modules.StudyPlanner.Application.Services;

/// <summary>Pure study-time arithmetic (Requirements.md §5.3). No clock, no I/O.</summary>
public static class StudyMath
{
    public static int RecommendedMinutes(int goalCount, int minutesPerGoal)
        => Math.Max(goalCount, 0) * Math.Max(minutesPerGoal, 0);

    /// <summary>Total study time spread over the remaining days; the divisor is clamped to at least 1.</summary>
    public static int DailyMinutes(int totalMinutes, DateOnly examDate, DateOnly today)
    {
        var days = Math.Max(examDate.DayNumber - today.DayNumber, 1);
        return Math.Max(totalMinutes, 0) / days;
    }

    /// <summary>Days until the exam; negative once the exam date has passed.</summary>
    public static int DaysRemaining(DateOnly examDate, DateOnly today)
        => examDate.DayNumber - today.DayNumber;
}
