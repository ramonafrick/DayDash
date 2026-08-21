using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Storage.Domain;

namespace DayDash.Modules.Storage.Application.Contracts
{
    public interface IExportService
    {
        Task ExportToIcsAsync(IEnumerable<CalendarEvent> events, string filePath, CancellationToken ct = default);
    }
}