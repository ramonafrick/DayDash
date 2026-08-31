using DayDash.Modules.Reminder.Domain;

namespace DayDash.Modules.Reminder.UI.Components;

public partial class ReminderSettingsComponent
{
    private TimeOnly _dailyTime = new(15, 30);
    private int _daysBefore = 1;
    private bool _enabled = true;
    private bool _saving;
    private bool _saved;

    protected override async Task OnInitializedAsync()
    {
        var config = await ReminderService.GetConfigAsync();
        _dailyTime = config.DailyStudyReminderTime;
        _daysBefore = config.EventReminderDaysBefore;
        _enabled = config.IsEnabled;
    }

    private async Task SaveAsync()
    {
        if (_saving)
        {
            return;
        }

        _saving = true;
        _saved = false;
        try
        {
            await ReminderService.SaveConfigAsync(new ReminderConfig
            {
                DailyStudyReminderTime = _dailyTime,
                EventReminderDaysBefore = _daysBefore < 0 ? 0 : _daysBefore,
                IsEnabled = _enabled,
            });
            _saved = true;
        }
        finally
        {
            _saving = false;
        }
    }
}
