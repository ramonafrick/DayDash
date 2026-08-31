using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Services;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class ExamDetailComponent
{
    [Inject] private IStudyPlannerService StudyPlanner { get; set; } = default!;
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private TimeProvider Time { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    /// <summary>Route parameter: the exam to show.</summary>
    [Parameter] public Guid ExamId { get; set; }

    /// <summary>Optional: caller can pass the exam directly instead of a load by id.</summary>
    [Parameter] public Exam? Exam { get; set; }

    private Exam? _exam;
    private Guid _loadedId;
    private bool _editing;
    private bool _confirmingDelete;

    private DateOnly Today => DateOnly.FromDateTime(Time.GetLocalNow().Date);

    private int DaysRemaining => _exam is null ? 0 : StudyMath.DaysRemaining(_exam.ExamDate, Today);

    protected override async Task OnParametersSetAsync()
    {
        if (Exam is not null)
        {
            _exam = Exam;
            return;
        }

        if (ExamId != _loadedId || _exam is null)
        {
            _loadedId = ExamId;
            _exam = await StudyPlanner.GetExamAsync(ExamId);
        }
    }

    private Task ToggleGoalAsync(LearningGoal goal) => StudyPlanner.SetGoalCheckedAsync(goal.Id, goal.IsChecked);

    private void StartEdit() => _editing = true;

    private async Task OnEditSavedAsync(Guid _)
    {
        _editing = false;
        _exam = await StudyPlanner.GetExamAsync(ExamId);
    }

    private void CancelEdit() => _editing = false;

    private async Task ConfirmDeleteAsync()
    {
        _confirmingDelete = false;
        await StudyPlanner.DeleteExamAsync(ExamId);
        Nav.NavigateTo("study");
    }
}
