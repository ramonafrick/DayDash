using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class EventListComponent
{
    [Parameter] public List<CalendarEvent> Events { get; set; } = new();
    [Parameter] public EventCallback<CalendarEvent> OnEventSelected { get; set; }
}