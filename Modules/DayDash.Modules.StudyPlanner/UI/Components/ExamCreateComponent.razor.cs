using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class ExamCreateComponent
{
    private Exam _exam = new();
    private List<SubjectConfig> _subjects = new();
    private int _recommendedMinutes;

    [Inject] private IStudyPlannerService StudyPlannerService { get; set; } = default!;
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        _subjects = (await StudyPlannerService.GetSubjectConfigsAsync()).ToList();
    }

    private void UpdateRecommendation()
    {
        var subject = _subjects.FirstOrDefault(s => s.Name == _exam.Subject);
        _recommendedMinutes = StudyPlannerService.CalculateRecommendedMinutes(_exam.LearningGoals.Count, subject?.Name ?? "");
    }

    private async Task HandleValidSubmit()
    {
        await StudyPlannerService.CreateExamAsync(_exam);
        _exam = new(); // Reset form
    }
}