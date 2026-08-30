using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
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
        try
        {
            TodayPlans = (await StudyPlannerService.GetTodayStudyPlanAsync()).ToList();
        }
        catch (InvalidOperationException)
        {
            // No persisted study data yet (model not built until the Storage slice). Show the empty state.
            TodayPlans = [];
        }
    }
}