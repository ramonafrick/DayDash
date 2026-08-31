using Bunit;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Camera.Application.Models;
using DayDash.Modules.Camera.Application.Services;
using DayDash.Modules.Camera.UI.Components;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DayDash.Tests.Camera;

public class CameraCaptureComponentTests : CultureIsolatedTest
{
    private static DayDashTestContext NewContext(out FakeCameraService camera, out FakeStudyPlannerService planner)
    {
        camera = new FakeCameraService();
        var subjects = new FakeSubjectConfigService();
        planner = new FakeStudyPlannerService(subjects);

        var ctx = new DayDashTestContext();
        ctx.Services.AddSingleton<ICameraService>(camera);
        ctx.Services.AddSingleton<ILearningGoalParser, LearningGoalParser>();
        ctx.Services.AddSingleton<ISubjectConfigService>(subjects);
        ctx.Services.AddSingleton<IStudyPlannerService>(planner);
        return ctx;
    }

    [Fact]
    public void Without_any_exam_it_shows_the_hint_and_no_capture_button()
    {
        using var ctx = NewContext(out _, out _);

        var cut = ctx.RenderComponent<CameraCaptureComponent>();

        Assert.Contains("Lege zuerst eine Prüfung an", cut.Markup);
        Assert.DoesNotContain("Foto aufnehmen", cut.Markup);
    }

    [Fact]
    public void No_text_found_shows_the_nothing_recognised_hint_and_no_rows()
    {
        using var ctx = NewContext(out var camera, out var planner);
        planner.Exams.Add(TestData.AnExam());
        camera.Next = OcrResult.Failure(OcrCaptureStatus.NoTextFound);
        var cut = ctx.RenderComponent<CameraCaptureComponent>();

        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.Contains("kein Text erkannt", cut.Markup));
        Assert.Empty(cut.FindAll("input[type=text]"));
    }

    [Fact]
    public void Cancelled_capture_shows_the_cancelled_hint_without_throwing()
    {
        using var ctx = NewContext(out var camera, out var planner);
        planner.Exams.Add(TestData.AnExam());
        camera.Next = OcrResult.Failure(OcrCaptureStatus.Cancelled);
        var cut = ctx.RenderComponent<CameraCaptureComponent>();

        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.Contains("Aufnahme abgebrochen", cut.Markup));
    }

    [Fact]
    public void Permission_denied_shows_the_permission_hint()
    {
        using var ctx = NewContext(out var camera, out var planner);
        planner.Exams.Add(TestData.AnExam());
        camera.Next = OcrResult.Failure(OcrCaptureStatus.PermissionDenied);
        var cut = ctx.RenderComponent<CameraCaptureComponent>();

        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.Contains("Kein Zugriff auf die Kamera", cut.Markup));
    }

    [Fact]
    public void Successful_recognition_renders_one_editable_row_per_line()
    {
        using var ctx = NewContext(out var camera, out var planner);
        planner.Exams.Add(TestData.AnExam());
        camera.Next = OcrResult.Success("Kapitel 1\nKapitel 2\nKapitel 3");
        var cut = ctx.RenderComponent<CameraCaptureComponent>();

        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.Equal(3, cut.FindAll("input[type=text]").Count));
    }

    [Fact]
    public void Saving_recognised_goals_calls_the_planner_and_confirms()
    {
        using var ctx = NewContext(out var camera, out var planner);
        var exam = TestData.AnExam();
        planner.Exams.Add(exam);
        camera.Next = OcrResult.Success("Kapitel 1\nKapitel 2");
        var cut = ctx.RenderComponent<CameraCaptureComponent>();
        cut.Find("button.btn-primary").Click();

        cut.WaitForAssertion(() => Assert.NotEmpty(cut.FindAll("input[type=text]")));
        cut.FindAll("button").First(b => b.TextContent.Contains("Ziele speichern")).Click();

        cut.WaitForAssertion(() =>
        {
            Assert.Equal(exam.Id, planner.LastSavedGoalsExamId);
            Assert.Equal(2, planner.LastSavedGoals!.Count);
            Assert.Contains("Lernziele gespeichert", cut.Markup);
        });
    }
}
