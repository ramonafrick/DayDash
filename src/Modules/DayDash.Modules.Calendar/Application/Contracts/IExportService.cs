using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Application.Contracts;

public interface IExportService
{
    Task ExportToIcsAsync(IEnumerable<CalendarEvent> events, string filePath, CancellationToken ct = default);
}
