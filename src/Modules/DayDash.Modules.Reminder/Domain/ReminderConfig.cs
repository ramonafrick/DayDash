namespace DayDash.Modules.Reminder.Domain;

public class ReminderConfig
{
    public Guid Id { get; set; }
    public TimeOnly DailyStudyReminderTime { get; set; } = new TimeOnly(15, 30);
    public int EventReminderDaysBefore { get; set; } = 1;
    public bool IsEnabled { get; set; }
}