using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Services;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class ExamListComponent
{
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private IStudyPlannerService StudyPlanner { get; set; } = default!;
    [Inject] private TimeProvider Time { get; set; } = default!;
    [Inject] private NavigationManager Nav { get; set; } = default!;

    private IReadOnlyList<Exam> _exams = [];
    private bool _creating;

    private DateOnly Today => DateOnly.FromDateTime(Time.GetLocalNow().Date);

    protected override async Task OnInitializedAsync() => await ReloadAsync();

    private async Task ReloadAsync() => _exams = await StudyPlanner.GetExamsAsync();

    private int DaysRemaining(Exam e) => StudyMath.DaysRemaining(e.ExamDate, Today);

    private void Open(Exam e) => Nav.NavigateTo($"study/exams/{e.Id}");

    private void StartCreate() => _creating = true;

    private async Task OnCreatedAsync(Guid id)
    {
        _creating = false;
        await ReloadAsync();
        Nav.NavigateTo($"study/exams/{id}");
    }

    private void CancelCreate() => _creating = false;
}
