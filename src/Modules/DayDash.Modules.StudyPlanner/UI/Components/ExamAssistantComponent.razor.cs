using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class ExamAssistantComponent
{
    [Inject] private IStudyPlannerService StudyPlanner { get; set; } = default!;
    [Inject] private ISubjectConfigService Subjects { get; set; } = default!;
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private TimeProvider Time { get; set; } = default!;

    /// <summary>Pre-fill from the calendar "Prüfung" event.</summary>
    [Parameter] public string? InitialTitle { get; set; }
    [Parameter] public DateOnly? InitialDate { get; set; }

    [Parameter] public EventCallback<Guid> OnExamCreated { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private int _step = 1;
    private List<SubjectConfig> _subjects = [];
    private readonly List<LearningGoal> _goals = [];

    private string _title = string.Empty;
    private string _subject = string.Empty;
    private DateOnly _date;
    private int _totalMinutes;
    private int _recommended;

    private bool _busy;

    private bool CanAdvanceStep1 => !string.IsNullOrWhiteSpace(_title) && !string.IsNullOrWhiteSpace(_subject);

    protected override async Task OnInitializedAsync()
    {
        _subjects = (await Subjects.GetAllAsync()).ToList();
        _title = InitialTitle ?? string.Empty;
        _date = InitialDate ?? DateOnly.FromDateTime(Time.GetLocalNow().Date).AddDays(7);
    }

    private void Next()
    {
        if (_step < 3)
        {
            _step++;
        }
    }

    private void Back()
    {
        if (_step > 1)
        {
            _step--;
        }
    }

    private async Task OnGoalsChangedAsync()
        => _recommended = await StudyPlanner.CalculateRecommendedMinutesAsync(GoalTexts().Count, _subject);

    private List<LearningGoal> GoalTexts() => _goals.Where(g => !string.IsNullOrWhiteSpace(g.Text)).ToList();

    private async Task FinishAsync()
    {
        if (_busy)
        {
            return;
        }

        _busy = true;
        var exam = new Exam
        {
            Id = Guid.NewGuid(),
            Title = _title.Trim(),
            Subject = _subject,
            ExamDate = _date,
            TotalStudyMinutes = _totalMinutes,
            LearningGoals = GoalTexts()
                .Select((g, i) => new LearningGoal { Id = Guid.NewGuid(), Text = g.Text.Trim(), SortOrder = i })
                .ToList(),
        };

        var id = await StudyPlanner.CreateExamAsync(exam);
        await OnExamCreated.InvokeAsync(id);
    }
}
