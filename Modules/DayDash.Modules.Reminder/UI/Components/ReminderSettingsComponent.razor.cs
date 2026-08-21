using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Reminder.Domain;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Reminder.UI.Components;

public partial class ReminderSettingsComponent
{
    [Inject] private IReminderService ReminderService { get; set; } = default!;
    [Inject] private IStringLocalizer<ReminderResources> Loc { get; set; } = default!;

    private TimeOnly DailyStudyReminderTime { get; set; } = new TimeOnly(15, 30);
    private int EventReminderDaysBefore { get; set; } = 1;
    private bool IsEnabled { get; set; } = true;

    private async Task SaveSettings()
    {
        var config = new ReminderConfig
        {
            DailyStudyReminderTime = DailyStudyReminderTime,
            EventReminderDaysBefore = EventReminderDaysBefore,
            IsEnabled = IsEnabled
        };

        await ReminderService.SaveConfigAsync(config);
    }
}