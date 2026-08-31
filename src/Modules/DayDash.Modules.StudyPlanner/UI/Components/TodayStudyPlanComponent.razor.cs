using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class TodayStudyPlanComponent
{
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private IStudyPlannerService StudyPlanner { get; set; } = default!;

    private IReadOnlyList<Exam> _plan = [];
    private int _totalMinutes;

    protected override async Task OnInitializedAsync()
    {
        _plan = await StudyPlanner.GetTodayStudyPlanAsync();
        _totalMinutes = _plan.Sum(e => e.DailyMinutes);
    }
}
