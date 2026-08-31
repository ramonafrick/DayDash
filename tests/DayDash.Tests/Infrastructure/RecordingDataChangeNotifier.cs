using DayDash.Modules.Storage.Application.Contracts;

namespace DayDash.Tests.Infrastructure;

public sealed class RecordingDataChangeNotifier : IDataChangeNotifier
{
    public List<DataChange> Changes { get; } = [];

    public Task NotifyAsync(DataChange change, CancellationToken ct = default)
    {
        Changes.Add(change);
        return Task.CompletedTask;
    }
}

public sealed class NullExportService : DayDash.Modules.Calendar.Application.Contracts.IExportService
{
    public string? LastPath { get; private set; }

    public int Calls { get; private set; }

    public Task ExportToIcsAsync(IEnumerable<DayDash.Modules.Calendar.Domain.CalendarEvent> events, string filePath, CancellationToken ct = default)
    {
        Calls++;
        LastPath = filePath;
        return Task.CompletedTask;
    }
}
