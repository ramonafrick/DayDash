using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.Infrastructure.Seeding;

/// <summary>Seeds the six built-in event types on first run (Requirements.md §5.1).</summary>
public sealed class EventTypeSeeder(IStringLocalizer<CalendarResources> localizer) : IDataSeeder
{
    public int Order => 10;

    public async Task SeedAsync(DayDashDbContext context, CancellationToken ct = default)
    {
        if (await context.Set<EventTypeConfig>().AnyAsync(ct))
        {
            return;
        }

        foreach (var d in EventTypeConfig.Defaults)
        {
            context.Set<EventTypeConfig>().Add(new EventTypeConfig
            {
                Id = d.Id,
                Key = d.Key,
                Name = localizer[d.ResourceKey],
                Color = d.Color,
                IsDefault = true,
            });
        }

        await context.SaveChangesAsync(ct);
    }
}
