using System.Collections.Generic;
using DayDash.Modules.Storage.Domain;

namespace DayDash.Modules.Storage.Application.Models
{
    public class IcsExportRequest
    {
        public string FilePath { get; init; } = string.Empty;
        public IEnumerable<CalendarEvent> Events { get; init; } = new List<CalendarEvent>();
    }
}