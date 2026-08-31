namespace DayDash.Modules.Reminder.Domain;

/// <summary>Single-row configuration for the Reminder module (Requirements.md §5.4).</summary>
public class ReminderConfig
{
    /// <summary>Fixed id - there is only ever one row, seeded on first run.</summary>
    public static readonly Guid SingletonId = new("4d3e0e00-0000-4000-a000-000000000001");

    public Guid Id { get; set; } = SingletonId;

    public TimeOnly DailyStudyReminderTime { get; set; } = new(15, 30);

    public int EventReminderDaysBefore { get; set; } = 1;

    public bool IsEnabled { get; set; } = true;
}
