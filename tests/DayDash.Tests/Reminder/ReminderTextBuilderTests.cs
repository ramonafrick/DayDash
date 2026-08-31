using System.Globalization;
using DayDash.Modules.Reminder.Application.Services;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Xunit;

namespace DayDash.Tests.Reminder;

public class ReminderTextBuilderTests : CultureIsolatedTest
{
    private readonly ReminderTextBuilder _builder = new(ReminderHost.Localizer);

    private static Exam Exam(string subject, DateOnly date) => new() { Id = Guid.NewGuid(), Title = subject, Subject = subject, ExamDate = date };

    [Fact]
    public void Daily_body_one_exam_german()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");

        var body = _builder.DailyStudyBody([Exam("Mathematik", new DateOnly(2026, 3, 20))], 60);

        Assert.Equal("Lernen für Mathematik – 60 Min heute", body);
    }

    [Fact]
    public void Daily_body_one_exam_english()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en");

        var body = _builder.DailyStudyBody([Exam("Mathematik", new DateOnly(2026, 3, 20))], 60);

        Assert.Equal("Study for Mathematik – 60 min today", body);
    }

    [Fact]
    public void Daily_body_multiple_exams_names_the_nearest_and_counts_the_rest()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");

        var body = _builder.DailyStudyBody(
        [
            Exam("Deutsch", new DateOnly(2026, 3, 25)),
            Exam("Mathematik", new DateOnly(2026, 3, 20)),
        ], 90);

        Assert.Equal("Mathematik und 1 weitere – 90 Min heute", body);
    }

    [Fact]
    public void Daily_body_is_null_when_nothing_to_study()
    {
        Assert.Null(_builder.DailyStudyBody([], 0));
        Assert.Null(_builder.DailyStudyBody([Exam("Mathematik", new DateOnly(2026, 3, 20))], 0));
    }

    [Fact]
    public void A_format_hostile_subject_does_not_break_formatting()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");

        var body = _builder.DailyStudyBody([Exam("{0} {1} {2}", new DateOnly(2026, 3, 20))], 45);

        Assert.Equal("Lernen für {0} {1} {2} – 45 Min heute", body);
    }

    [Fact]
    public void Event_body_includes_the_title_and_the_localized_date()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");

        var body = _builder.EventBody("Zahnarzt", new DateOnly(2026, 4, 1));

        Assert.Contains("Zahnarzt", body);
        Assert.Contains("2026", body);
    }
}
