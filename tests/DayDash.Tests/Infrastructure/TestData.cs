using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Tests.Infrastructure;

/// <summary>Builders for domain entities used across tests. Extended per slice.</summary>
public static class TestData
{
    public static CalendarEvent AnEvent(
        string title = "Test event",
        DateOnly? date = null,
        TimeOnly? from = null,
        TimeOnly? to = null,
        bool allDay = false) => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        Date = date ?? new DateOnly(2026, 3, 10),
        TimeFrom = from,
        TimeTo = to,
        IsAllDay = allDay,
    };

    public static Exam AnExam(
        string title = "Test exam",
        string subject = "Mathematik",
        DateOnly? examDate = null,
        int totalStudyMinutes = 120) => new()
    {
        Id = Guid.NewGuid(),
        Title = title,
        Subject = subject,
        ExamDate = examDate ?? new DateOnly(2026, 3, 20),
        TotalStudyMinutes = totalStudyMinutes,
    };
}
