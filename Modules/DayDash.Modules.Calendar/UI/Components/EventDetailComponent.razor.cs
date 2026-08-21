using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class EventDetailComponent
{
    [Parameter] public required CalendarEvent Event { get; set; }
    [Parameter] public EventCallback<CalendarEvent> OnEdit { get; set; }
    [Parameter] public EventCallback<CalendarEvent> OnDelete { get; set; }

    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;
}