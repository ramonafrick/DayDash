using DayDash.Modules.Calendar.Application.Contracts;
using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Domain;

namespace DayDash.Modules.Calendar.Infrastructure;

public sealed class IcsExportService : IExportService
{
    public async Task ExportToIcsAsync(IEnumerable<CalendarEvent> events, string filePath, CancellationToken ct = default)
        => await File.WriteAllTextAsync(filePath, IcsBuilder.Build(events), ct);
}
