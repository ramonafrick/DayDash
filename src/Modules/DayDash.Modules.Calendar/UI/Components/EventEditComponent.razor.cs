using System.ComponentModel.DataAnnotations;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class EventEditComponent
{
    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;
    [Inject] private TimeProvider Time { get; set; } = default!;

    /// <summary>Null to create a new event.</summary>
    [Parameter] public CalendarEvent? Event { get; set; }

    [Parameter] public IReadOnlyList<EventTypeConfig> EventTypes { get; set; } = [];

    [Parameter] public EventCallback<CalendarEvent> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private EditModel _model = new();
    private Guid? _boundEventId;
    private bool _bound;
    private bool IsNew => Event is null;

    protected override void OnParametersSet()
    {
        // Rebind when the target event changes (incl. edit -> "New event" while the panel is open).
        if (_bound && Event?.Id == _boundEventId)
        {
            return;
        }

        _bound = true;
        _boundEventId = Event?.Id;
        _model = new EditModel();

        if (Event is null)
        {
            _model.Date = DateOnly.FromDateTime(Time.GetLocalNow().Date);
            return;
        }

        _model.Title = Event.Title;
        _model.EventTypeId = Event.EventTypeId;
        _model.Date = Event.Date;
        _model.IsAllDay = Event.IsAllDay;
        _model.TimeFrom = Event.TimeFrom;
        _model.TimeTo = Event.TimeTo;
        _model.Notes = Event.Notes;
    }

    private async Task SubmitAsync()
    {
        // Build a detached copy so a failed save never leaves half-applied values in the caller's list.
        var result = new CalendarEvent
        {
            Id = Event?.Id ?? Guid.NewGuid(),
            LinkedExamId = Event?.LinkedExamId,
            Title = _model.Title.Trim(),
            EventTypeId = _model.EventTypeId,
            Date = _model.Date,
            IsAllDay = _model.IsAllDay,
            TimeFrom = _model.IsAllDay ? null : _model.TimeFrom,
            TimeTo = _model.IsAllDay ? null : _model.TimeTo,
            Notes = string.IsNullOrWhiteSpace(_model.Notes) ? null : _model.Notes.Trim(),
        };

        await OnSave.InvokeAsync(result);
    }

    private sealed class EditModel : IValidatableObject
    {
        [Required(ErrorMessage = "TitleRequired")]
        public string Title { get; set; } = string.Empty;

        public Guid? EventTypeId { get; set; }

        public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);

        public bool IsAllDay { get; set; }

        public TimeOnly? TimeFrom { get; set; }

        public TimeOnly? TimeTo { get; set; }

        public string? Notes { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!IsAllDay && TimeFrom is { } from && TimeTo is { } to && to < from)
            {
                yield return new ValidationResult("TimeToBeforeTimeFrom", [nameof(TimeTo)]);
            }
        }
    }
}
