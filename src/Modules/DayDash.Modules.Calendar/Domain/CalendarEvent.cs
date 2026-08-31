namespace DayDash.Modules.Calendar.Domain;

public class CalendarEvent
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    /// <summary>Optional link to a configured <see cref="EventTypeConfig"/>. Null once the type is deleted.</summary>
    public Guid? EventTypeId { get; set; }

    public EventTypeConfig? EventType { get; set; }

    public DateOnly Date { get; set; }

    public TimeOnly? TimeFrom { get; set; }

    public TimeOnly? TimeTo { get; set; }

    public string? Notes { get; set; }

    public bool IsAllDay { get; set; }

    /// <summary>Set for "Prüfung" events created through the exam assistant (StudyPlanner).</summary>
    public Guid? LinkedExamId { get; set; }
}
