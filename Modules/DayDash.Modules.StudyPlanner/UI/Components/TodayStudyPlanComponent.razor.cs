using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class TodayStudyPlanComponent
{
    private List<Exam> TodayPlans { get; set; } = new();

    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private IStudyPlannerService StudyPlannerService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        TodayPlans = (await StudyPlannerService.GetTodayStudyPlanAsync()).ToList();
    }
}