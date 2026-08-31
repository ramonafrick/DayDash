using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Application.Models;
using DayDash.Modules.Reminder.Application.Services;
using DayDash.Modules.Reminder.Domain;
using DayDash.Modules.Reminder.Resources;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Time.Testing;

namespace DayDash.Tests.Infrastructure;

/// <summary>Wires <see cref="ReminderService"/> against in-memory fakes for logic tests.</summary>
public sealed class ReminderHost
{
    public ReminderHost(DateTimeOffset? now = null)
    {
        Time = new FakeTimeProvider(now ?? new DateTimeOffset(2026, 3, 10, 8, 0, 0, TimeSpan.Zero));
        Subjects = new FakeSubjectConfigService();
        StudyPlanner = new FakeStudyPlannerService(Subjects);
        TextBuilder = new ReminderTextBuilder(Localizer);
        Service = new ReminderService(Config, Scheduler, TextBuilder, StudyPlanner, Calendar, Time);
    }

    public static IStringLocalizer<ReminderResources> Localizer { get; } =
        new ServiceCollection().AddLogging().AddLocalization().BuildServiceProvider()
            .GetRequiredService<IStringLocalizer<ReminderResources>>();

    public FakeReminderConfigRepository Config { get; } = new();
    public RecordingNotificationScheduler Scheduler { get; } = new();
    public FakeSubjectConfigService Subjects { get; }
    public FakeStudyPlannerService StudyPlanner { get; }
    public FakeCalendarService Calendar { get; } = new();
    public FakeTimeProvider Time { get; }
    public ReminderTextBuilder TextBuilder { get; }
    public ReminderService Service { get; }
}

public sealed class FakeReminderService : IReminderService
{
    public bool NotificationsSupported { get; set; } = true;

    public ReminderConfig Config { get; set; } = new();

    public ReminderConfig? LastSaved { get; private set; }

    public int Reschedules { get; private set; }

    public Task<ReminderConfig> GetConfigAsync(CancellationToken ct = default) => Task.FromResult(Config);

    public Task SaveConfigAsync(ReminderConfig config, CancellationToken ct = default)
    {
        LastSaved = config;
        Config = config;
        return Task.CompletedTask;
    }

    public Task RescheduleAllAsync(CancellationToken ct = default)
    {
        Reschedules++;
        return Task.CompletedTask;
    }
}

public sealed class FakeReminderConfigRepository : IReminderConfigRepository
{
    public ReminderConfig Config { get; set; } = new();

    public int Saves { get; private set; }

    public Task<ReminderConfig> GetAsync(CancellationToken ct = default) => Task.FromResult(Config);

    public Task SaveAsync(ReminderConfig config, CancellationToken ct = default)
    {
        Config = new ReminderConfig
        {
            Id = ReminderConfig.SingletonId,
            DailyStudyReminderTime = config.DailyStudyReminderTime,
            EventReminderDaysBefore = config.EventReminderDaysBefore,
            IsEnabled = config.IsEnabled,
        };
        Saves++;
        return Task.CompletedTask;
    }
}

public sealed class RecordingNotificationScheduler : INotificationScheduler
{
    public bool IsSupported => true;

    public List<NotificationRequest> Scheduled { get; } = [];

    public List<int> Cancelled { get; } = [];

    public int CancelAllCount { get; private set; }

    public bool PermissionGranted { get; set; } = true;

    public NotificationRequest? Daily =>
        Scheduled.FirstOrDefault(r => r.Id == NotificationIds.DailyStudyReminder);

    public Task ScheduleAsync(NotificationRequest request, CancellationToken ct = default)
    {
        Scheduled.RemoveAll(r => r.Id == request.Id); // re-scheduling replaces
        Scheduled.Add(request);
        return Task.CompletedTask;
    }

    public Task CancelAsync(int notificationId, CancellationToken ct = default)
    {
        Cancelled.Add(notificationId);
        Scheduled.RemoveAll(r => r.Id == notificationId);
        return Task.CompletedTask;
    }

    public Task CancelAllAsync(CancellationToken ct = default)
    {
        CancelAllCount++;
        Scheduled.Clear();
        return Task.CompletedTask;
    }

    public Task<bool> RequestPermissionAsync(CancellationToken ct = default) => Task.FromResult(PermissionGranted);
}
