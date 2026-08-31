using System.Globalization;
using DayDash.Modules.Reminder.Resources;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Reminder.Application.Services;

/// <summary>
/// Builds the localized title/body for reminder notifications. Pure - it formats strings
/// under the current culture and does no scheduling.
/// </summary>
public sealed class ReminderTextBuilder(IStringLocalizer<ReminderResources> loc)
{
    public string DailyStudyTitle => loc["DailyStudyReminderTitle"];

    /// <summary>
    /// Body for the daily study reminder, or <c>null</c> when there is nothing to study today
    /// (the caller then schedules no daily reminder - FR-R2).
    /// </summary>
    public string? DailyStudyBody(IReadOnlyList<Exam> todaysExams, int totalMinutes)
    {
        if (todaysExams.Count == 0 || totalMinutes <= 0)
        {
            return null;
        }

        var next = todaysExams.OrderBy(e => e.ExamDate).First();
        return todaysExams.Count == 1
            ? string.Format(CultureInfo.CurrentCulture, loc["DailyStudyReminderBody"], next.Subject, totalMinutes)
            : string.Format(CultureInfo.CurrentCulture, loc["DailyStudyReminderBodyMultiple"], next.Subject, totalMinutes, todaysExams.Count - 1);
    }

    public string EventTitle => loc["EventReminderTitle"];

    public string EventBody(string eventTitle, DateOnly eventDate)
        => string.Format(CultureInfo.CurrentCulture, loc["EventReminderBody"], eventTitle, eventDate.ToString("d", CultureInfo.CurrentCulture));
}
