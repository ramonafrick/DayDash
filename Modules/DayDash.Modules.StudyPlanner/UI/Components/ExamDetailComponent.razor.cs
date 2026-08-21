using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class ExamDetailComponent
{
    [Parameter] public required Exam Exam { get; set; }
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Parameter] public EventCallback<Exam> OnEdit { get; set; }
    [Parameter] public EventCallback<Guid> OnDelete { get; set; }

    private int DaysRemaining => (Exam.ExamDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

    private async Task EditExam()
    {
        await OnEdit.InvokeAsync(Exam);
    }

    private async Task DeleteExam()
    {
        await OnDelete.InvokeAsync(Exam.Id);
    }
}