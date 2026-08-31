using DayDash.Modules.Reminder.Domain;

namespace DayDash.Modules.Reminder.Application.Contracts;

public interface IReminderConfigRepository
{
    Task<ReminderConfig> GetAsync(CancellationToken ct = default);

    Task SaveAsync(ReminderConfig config, CancellationToken ct = default);
}
