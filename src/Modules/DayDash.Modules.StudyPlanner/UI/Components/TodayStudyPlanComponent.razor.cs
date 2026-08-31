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

    [Parameter] public int RefreshToken { get; set; }

    private int _lastRefreshToken;

    protected override async Task OnInitializedAsync() => await ReloadAsync();

    protected override async Task OnParametersSetAsync()
    {
        if (RefreshToken != _lastRefreshToken)
        {
            _lastRefreshToken = RefreshToken;
            await ReloadAsync();
        }
    }

    private async Task ReloadAsync()
    {
        _plan = await StudyPlanner.GetTodayStudyPlanAsync();
        _totalMinutes = _plan.Sum(e => e.DailyMinutes);
    }
}
