using System.Collections.Generic;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Application.Models;

public class IcsExportRequest
{
    public string FilePath { get; init; } = string.Empty;
    public IEnumerable<CalendarEvent> Events { get; init; } = new List<CalendarEvent>();
}
