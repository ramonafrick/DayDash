using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Application.Models;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Settings.Application.Contracts;
using DayDash.Modules.Settings.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class CalendarComponent
{
    private enum Panel { None, DayList, Detail, Edit }

    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;
    [Inject] private ICalendarService Calendar { get; set; } = default!;
    [Inject] private IFileShareService FileShare { get; set; } = default!;

    /// <summary>Host-supplied exam assistant, shown when a "Prüfung" event is created (FR-C6; wired in Slice 3).</summary>
    [Parameter] public RenderFragment<ExamAssistantRequest>? ExamAssistantTemplate { get; set; }

    /// <summary>Raised when the user asks to open the exam linked to an event.</summary>
    [Parameter] public EventCallback<Guid> OnOpenLinkedExam { get; set; }

    private bool _showWeek;
    private int _refreshToken;

    private Panel _panel = Panel.None;
    private DateOnly _selectedDay;
    private IReadOnlyList<CalendarEvent> _dayEvents = [];
    private CalendarEvent? _selectedEvent;
    private CalendarEvent? _editEvent;
    private IReadOnlyList<EventTypeConfig> _eventTypes = [];

    private CalendarEvent? _pendingDelete;
    private string? _toast;

    protected override async Task OnInitializedAsync()
        => _eventTypes = await Calendar.GetEventTypesAsync();

    private void ShowMonth()
    {
        _showWeek = false;
        _toast = null;
    }

    private void ShowWeek()
    {
        _showWeek = true;
        _toast = null;
    }

    private async Task OnDaySelectedAsync(DateOnly date)
    {
        _toast = null;
        _selectedDay = date;
        _dayEvents = await Calendar.GetEventsForDayAsync(date);
        _panel = Panel.DayList;
    }

    private void OnEventSelected(CalendarEvent e)
    {
        _selectedEvent = e;
        _panel = Panel.Detail;
    }

    private async Task StartCreateAsync()
    {
        _toast = null;
        _eventTypes = await Calendar.GetEventTypesAsync();
        _editEvent = null;
        _panel = Panel.Edit;
    }

    private async Task StartEditAsync(CalendarEvent e)
    {
        _toast = null;
        _eventTypes = await Calendar.GetEventTypesAsync();
        _editEvent = e;
        _panel = Panel.Edit;
    }

    private async Task SaveAsync(CalendarEvent e)
    {
        if (await Calendar.GetEventAsync(e.Id) is null)
        {
            await Calendar.CreateEventAsync(e);
        }
        else
        {
            await Calendar.UpdateEventAsync(e);
        }

        _refreshToken++;
        ClosePanel();
    }

    private void RequestDelete(CalendarEvent e) => _pendingDelete = e;

    private void CancelDelete() => _pendingDelete = null;

    private async Task ConfirmDeleteAsync()
    {
        if (_pendingDelete is not null)
        {
            await Calendar.DeleteEventAsync(_pendingDelete.Id);
            _pendingDelete = null;
            _refreshToken++;
            ClosePanel();
        }
    }

    private void ClosePanel()
    {
        _panel = Panel.None;
        _selectedEvent = null;
        _editEvent = null;
    }

    private async Task ExportAsync()
    {
        try
        {
            var path = Path.Combine(Path.GetTempPath(), $"DayDash-{DateTime.UtcNow:yyyyMMdd-HHmmss}.ics");
            await Calendar.ExportIcsAsync(path);
            await FileShare.ShareFileAsync(path, Loc["ExportIcs"]);
            _toast = Loc["ExportSuccess"];
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or InvalidOperationException)
        {
            _toast = Loc["ExportFailed"];
        }
    }
}
