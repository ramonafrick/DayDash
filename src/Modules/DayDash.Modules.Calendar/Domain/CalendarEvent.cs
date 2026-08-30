using System;

namespace DayDash.Modules.Calendar.Domain;

public class CalendarEvent
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public TimeOnly? TimeFrom { get; set; }
    public TimeOnly? TimeTo { get; set; }
    public string? Notes { get; set; }
    public bool IsAllDay { get; set; }
    public Guid? LinkedExamId { get; set; }
}