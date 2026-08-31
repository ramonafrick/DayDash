using DayDash.Modules.Reminder.Domain;

namespace DayDash.Modules.Reminder.Application.Contracts;

/// <summary>
/// Module-level orchestrator for reminders: owns the single <see cref="ReminderConfig"/> and
/// turns the current data (today's study load, upcoming events) into scheduled notifications.
/// </summary>
public interface IReminderService
{
    Task<ReminderConfig> GetConfigAsync(CancellationToken ct = default);

    /// <summary>Persists the config and re-computes the whole schedule.</summary>
    Task SaveConfigAsync(ReminderConfig config, CancellationToken ct = default);

    /// <summary>Cancels every scheduled reminder and re-schedules from scratch (FR-R6). Safe to call often.</summary>
    Task RescheduleAllAsync(CancellationToken ct = default);
}
