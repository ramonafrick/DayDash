using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.UI.Components;

public partial class SubjectConfigComponent
{
    [Inject] private IStringLocalizer<StudyPlannerResources> Loc { get; set; } = default!;
    [Inject] private ISubjectConfigService Subjects { get; set; } = default!;

    private readonly List<SubjectConfig> _subjects = [];
    private string? _toast;

    protected override async Task OnInitializedAsync() => await ReloadAsync();

    private async Task ReloadAsync()
    {
        _subjects.Clear();
        _subjects.AddRange(await Subjects.GetAllAsync());
    }

    private async Task SaveAsync(SubjectConfig subject)
    {
        await Subjects.SaveAsync(subject);
        _toast = Loc["Saved"];
    }

    private async Task AddAsync()
    {
        var created = new SubjectConfig { Id = Guid.NewGuid(), Name = Loc["NewSubject"], MinutesPerGoal = SubjectConfig.DefaultMinutesPerGoal };
        await Subjects.SaveAsync(created);
        await ReloadAsync();
    }

    private async Task DeleteAsync(SubjectConfig subject)
    {
        await Subjects.DeleteAsync(subject.Id);
        await ReloadAsync();
    }
}
