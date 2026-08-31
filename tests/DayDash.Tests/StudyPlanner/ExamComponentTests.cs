using Bunit;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.UI.Components;
using DayDash.Tests.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DayDash.Tests.StudyPlanner;

public class ExamComponentTests : CultureIsolatedTest
{
    private static DayDashTestContext NewContext(out FakeStudyPlannerService planner, out FakeSubjectConfigService subjects)
    {
        subjects = new FakeSubjectConfigService();
        planner = new FakeStudyPlannerService(subjects);
        var ctx = new DayDashTestContext();
        ctx.Services.AddSingleton<ISubjectConfigService>(subjects);
        ctx.Services.AddSingleton<IStudyPlannerService>(planner);
        return ctx;
    }

    [Fact]
    public void ExamCreate_updates_the_recommendation_when_a_goal_is_added_and_when_the_subject_changes()
    {
        using var ctx = NewContext(out _, out _);
        var cut = ctx.RenderComponent<ExamCreateComponent>();

        cut.Find("#ex-subject").Change("Deutsch"); // 20 min/goal
        cut.FindAll("button").First(b => b.TextContent.Contains("Ziel")).Click(); // Add goal

        cut.WaitForAssertion(() => Assert.Contains("20 Minuten", cut.Markup));
    }

    [Fact]
    public void ExamCreate_empty_title_blocks_submit()
    {
        using var ctx = NewContext(out var planner, out _);
        var cut = ctx.RenderComponent<ExamCreateComponent>();

        cut.Find("#ex-subject").Change("Deutsch");
        cut.Find("form").Submit();

        Assert.Null(planner.LastCreated);
        Assert.Contains("Titel", cut.Markup);
    }

    [Fact]
    public void ExamAssistant_prefills_title_and_date_and_finishing_raises_OnExamCreated()
    {
        using var ctx = NewContext(out var planner, out _);
        Guid? created = null;
        var cut = ctx.RenderComponent<ExamAssistantComponent>(p => p
            .Add(c => c.InitialTitle, "Mathe-Prüfung")
            .Add(c => c.InitialDate, new DateOnly(2026, 4, 1))
            .Add(c => c.OnExamCreated, id => created = id));

        Assert.Equal("Mathe-Prüfung", cut.Find("#a-title").GetAttribute("value"));

        cut.Find("#a-subject").Change("Mathematik");
        cut.FindAll("button").First(b => b.TextContent.Contains("Weiter")).Click(); // -> goals
        cut.FindAll("button").First(b => b.TextContent.Contains("Weiter")).Click(); // -> minutes
        cut.FindAll("button").First(b => b.TextContent.Contains("Fertig")).Click();

        cut.WaitForAssertion(() =>
        {
            Assert.NotNull(created);
            Assert.Equal("Mathe-Prüfung", planner.LastCreated!.Title);
            Assert.Equal(new DateOnly(2026, 4, 1), planner.LastCreated.ExamDate);
        });
    }

    [Fact]
    public void ExamAssistant_cancel_does_not_create_anything()
    {
        using var ctx = NewContext(out var planner, out _);
        var cancelled = false;
        var cut = ctx.RenderComponent<ExamAssistantComponent>(p => p
            .Add(c => c.OnCancel, () => cancelled = true));

        cut.FindAll("button").First(b => b.TextContent.Contains("Abbrechen")).Click();

        Assert.True(cancelled);
        Assert.Null(planner.LastCreated);
    }

    [Fact]
    public void TodayStudyPlan_shows_the_empty_state_when_there_is_nothing_to_study()
    {
        using var ctx = NewContext(out _, out _);

        var cut = ctx.RenderComponent<TodayStudyPlanComponent>();

        Assert.Contains("Heute nichts zu lernen", cut.Markup);
        Assert.Empty(cut.FindAll("li"));
    }
}
