using DayDash.Modules.Reminder.Domain;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Reminder.Infrastructure.Seeding;

/// <summary>Seeds the single default reminder configuration on first run (15:30, 1 day, enabled).</summary>
public sealed class ReminderConfigSeeder : IDataSeeder
{
    public int Order => 30;

    public async Task SeedAsync(DayDashDbContext context, CancellationToken ct = default)
    {
        if (await context.Set<ReminderConfig>().AnyAsync(ct))
        {
            return;
        }

        context.Set<ReminderConfig>().Add(new ReminderConfig());
        await context.SaveChangesAsync(ct);
    }
}
