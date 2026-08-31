using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Settings.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class EventTypeSettingsComponent
{
    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;
    [Inject] private ICalendarService Calendar { get; set; } = default!;

    private readonly List<EventTypeConfig> _types = [];
    private EventTypeConfig? _pendingDelete;

    private string _newName = string.Empty;
    private string _newColor = "#4A90E2";

    protected override async Task OnInitializedAsync() => await ReloadAsync();

    private async Task ReloadAsync()
    {
        _types.Clear();
        _types.AddRange(await Calendar.GetEventTypesAsync());
    }

    private async Task AddAsync()
    {
        if (string.IsNullOrWhiteSpace(_newName))
        {
            return;
        }

        await Calendar.SaveEventTypeAsync(new EventTypeConfig
        {
            Id = Guid.NewGuid(),
            Key = string.Empty,
            Name = _newName.Trim(),
            Color = _newColor,
            IsDefault = false,
        });

        _newName = string.Empty;
        _newColor = "#4A90E2";
        await ReloadAsync();
    }

    private Task SaveAsync(EventTypeConfig type) => Calendar.SaveEventTypeAsync(type);

    private void RequestDelete(EventTypeConfig type) => _pendingDelete = type;

    private void CancelDelete() => _pendingDelete = null;

    private async Task ConfirmDeleteAsync()
    {
        if (_pendingDelete is not null)
        {
            await Calendar.DeleteEventTypeAsync(_pendingDelete.Id);
            _pendingDelete = null;
            await ReloadAsync();
        }
    }
}
