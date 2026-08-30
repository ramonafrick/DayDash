namespace DayDash.Modules.Reminder.Application.Models;

public class ReminderRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public DateTime ScheduledAt { get; set; }
    public Guid ReferenceId { get; set; }
}