using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class SubjectConfigComponent
{
    private List<SubjectConfig> Subjects { get; set; } = new();

    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private IStudyPlannerService StudyPlannerService { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        Subjects = (await StudyPlannerService.GetSubjectConfigsAsync()).ToList();
    }

    private void AddSubject()
    {
        Subjects.Add(new SubjectConfig { Id = Guid.NewGuid(), Name = Loc["NewSubject"], MinutesPerGoal = 15 });
    }

    private void DeleteSubject(SubjectConfig subject)
    {
        Subjects.Remove(subject);
    }

    private async Task SaveConfig()
    {
        foreach (var subject in Subjects)
        {
            await StudyPlannerService.SaveSubjectConfigAsync(subject);
        }
    }
}