using System.ComponentModel.DataAnnotations;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Settings.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class EventEditComponent
{
    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;

    /// <summary>Null to create a new event.</summary>
    [Parameter] public CalendarEvent? Event { get; set; }

    [Parameter] public IReadOnlyList<EventTypeConfig> EventTypes { get; set; } = [];

    [Parameter] public EventCallback<CalendarEvent> OnSave { get; set; }
    [Parameter] public EventCallback OnCancel { get; set; }

    private readonly EditModel _model = new();
    private bool IsNew => Event is null;

    protected override void OnParametersSet()
    {
        if (Event is null)
        {
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
        var target = Event ?? new CalendarEvent { Id = Guid.NewGuid() };
        target.Title = _model.Title.Trim();
        target.EventTypeId = _model.EventTypeId;
        target.EventType = null;
        target.Date = _model.Date;
        target.IsAllDay = _model.IsAllDay;
        target.TimeFrom = _model.IsAllDay ? null : _model.TimeFrom;
        target.TimeTo = _model.IsAllDay ? null : _model.TimeTo;
        target.Notes = string.IsNullOrWhiteSpace(_model.Notes) ? null : _model.Notes.Trim();

        await OnSave.InvokeAsync(target);
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
