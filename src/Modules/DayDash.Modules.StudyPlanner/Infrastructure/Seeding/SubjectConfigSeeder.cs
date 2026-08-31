using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Modules.StudyPlanner.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.StudyPlanner.Infrastructure.Seeding;

/// <summary>Seeds the five built-in subjects on first run (Requirements.md §5.3).</summary>
public sealed class SubjectConfigSeeder(IStringLocalizer<StudyPlannerResources> localizer) : IDataSeeder
{
    public int Order => 20;

    public async Task SeedAsync(DayDashDbContext context, CancellationToken ct = default)
    {
        if (await context.Set<SubjectConfig>().AnyAsync(ct))
        {
            return;
        }

        foreach (var d in SubjectConfig.Defaults)
        {
            context.Set<SubjectConfig>().Add(new SubjectConfig
            {
                Id = d.Id,
                Name = localizer[d.ResourceKey],
                MinutesPerGoal = SubjectConfig.DefaultMinutesPerGoal,
            });
        }

        await context.SaveChangesAsync(ct);
    }
}
