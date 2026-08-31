using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Domain;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Reminder.Infrastructure;

public sealed class ReminderConfigRepository(DayDashDbContext context) : IReminderConfigRepository
{
    public async Task<ReminderConfig> GetAsync(CancellationToken ct = default)
        => await context.Set<ReminderConfig>().FirstOrDefaultAsync(ct) ?? new ReminderConfig();

    public async Task SaveAsync(ReminderConfig config, CancellationToken ct = default)
    {
        var existing = await context.Set<ReminderConfig>().FindAsync([ReminderConfig.SingletonId], ct);
        if (existing is null)
        {
            context.Set<ReminderConfig>().Add(new ReminderConfig
            {
                Id = ReminderConfig.SingletonId,
                DailyStudyReminderTime = config.DailyStudyReminderTime,
                EventReminderDaysBefore = config.EventReminderDaysBefore,
                IsEnabled = config.IsEnabled,
            });
        }
        else
        {
            existing.DailyStudyReminderTime = config.DailyStudyReminderTime;
            existing.EventReminderDaysBefore = config.EventReminderDaysBefore;
            existing.IsEnabled = config.IsEnabled;
        }

        await context.SaveChangesAsync(ct);
    }
}
