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
        catch (InvalidOperationException ex) when (ex.Message.Contains("cannot be used for entity type", StringComparison.Ordinal)
                                                   || ex.Message.Contains("was not found in the model", StringComparison.Ordinal))
        {
            // The persistence model is not assembled until the Storage slice - show the empty state.
            // TODO(Slice 3): remove this guard once the StudyPlanner model exists.
            TodayPlans = [];
        }
    }
}