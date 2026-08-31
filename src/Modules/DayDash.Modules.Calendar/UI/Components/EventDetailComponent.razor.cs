using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;
using DayDash.Modules.Settings.UI;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class EventDetailComponent
{
    [Parameter, EditorRequired] public CalendarEvent Event { get; set; } = default!;
    [Parameter] public EventCallback<CalendarEvent> OnEdit { get; set; }
    [Parameter] public EventCallback<CalendarEvent> OnDelete { get; set; }
    [Parameter] public EventCallback<Guid> OnOpenLinkedExam { get; set; }

    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;
}
