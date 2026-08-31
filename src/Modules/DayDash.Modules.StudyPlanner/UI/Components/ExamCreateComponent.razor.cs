using System.ComponentModel.DataAnnotations;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class ExamCreateComponent
{
    [Inject] private IStudyPlannerService StudyPlanner { get; set; } = default!;
    [Inject] private ISubjectConfigService Subjects { get; set; } = default!;
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private TimeProvider Time { get; set; } = default!;

    /// <summary>Existing exam to edit; null to create.</summary>
    [Parameter] public Exam? Exam { get; set; }

    [Parameter] public EventCallback<Guid> OnSaved { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private readonly EditModel _model = new();
    private List<SubjectConfig> _subjects = [];
    private readonly List<LearningGoal> _goals = [];
    private int _recommendedMinutes;
    private bool IsNew => Exam is null;

    protected override async Task OnInitializedAsync()
    {
        _subjects = (await Subjects.GetAllAsync()).ToList();

        if (Exam is not null)
        {
            _model.Title = Exam.Title;
            _model.Subject = Exam.Subject;
            _model.ExamDate = Exam.ExamDate;
            _model.TotalStudyMinutes = Exam.TotalStudyMinutes;
            _goals.AddRange(Exam.LearningGoals.OrderBy(g => g.SortOrder)
                .Select(g => new LearningGoal { Id = g.Id, Text = g.Text, IsChecked = g.IsChecked, SortOrder = g.SortOrder }));
        }
        else
        {
            _model.ExamDate = DateOnly.FromDateTime(Time.GetLocalNow().Date).AddDays(7);
        }

        await UpdateRecommendationAsync();
    }

    private async Task UpdateRecommendationAsync()
        => _recommendedMinutes = await StudyPlanner.CalculateRecommendedMinutesAsync(_goals.Count, _model.Subject);

    private async Task OnGoalsChangedAsync() => await UpdateRecommendationAsync();

    private async Task SubmitAsync()
    {
        var exam = Exam ?? new Exam { Id = Guid.NewGuid() };
        exam.Title = _model.Title.Trim();
        exam.Subject = _model.Subject;
        exam.ExamDate = _model.ExamDate;
        exam.TotalStudyMinutes = _model.TotalStudyMinutes;
        exam.LearningGoals = _goals
            .Where(g => !string.IsNullOrWhiteSpace(g.Text))
            .Select((g, i) => new LearningGoal { Id = g.Id, ExamId = exam.Id, Text = g.Text.Trim(), IsChecked = g.IsChecked, SortOrder = i })
            .ToList();

        var id = IsNew ? await StudyPlanner.CreateExamAsync(exam) : await Persist(exam);
        await OnSaved.InvokeAsync(id);
    }

    private async Task<Guid> Persist(Exam exam)
    {
        await StudyPlanner.UpdateExamAsync(exam);
        return exam.Id;
    }

    private sealed class EditModel
    {
        [Required(ErrorMessage = "TitleRequired")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "SubjectRequired")]
        public string Subject { get; set; } = string.Empty;

        public DateOnly ExamDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        [Range(0, 10000, ErrorMessage = "MinutesRange")]
        public int TotalStudyMinutes { get; set; }
    }
}
