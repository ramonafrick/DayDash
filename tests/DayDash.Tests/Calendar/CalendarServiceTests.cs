using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Infrastructure;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.Calendar;

public class CalendarServiceTests
{
    private static (CalendarService service, RecordingDataChangeNotifier notifier) Build(SqliteDbContextFixture f)
    {
        var notifier = new RecordingDataChangeNotifier();
        var service = new CalendarService(new CalendarRepository(f.Context), new NullExportService(), notifier);
        return (service, notifier);
    }

    [Fact]
    public async Task Create_update_delete_each_raise_one_matching_DataChange()
    {
        await using var f = new SqliteDbContextFixture();
        var (service, notifier) = Build(f);
        var e = TestData.AnEvent();

        await service.CreateEventAsync(e);
        await service.UpdateEventAsync(e);
        await service.DeleteEventAsync(e.Id);

        Assert.Equal(
            new[]
            {
                DataChangeKind.CalendarEventSaved,
                DataChangeKind.CalendarEventSaved,
                DataChangeKind.CalendarEventDeleted,
            },
            notifier.Changes.Select(c => c.Kind));
        Assert.All(notifier.Changes, c => Assert.Equal(e.Id, c.EntityId));
    }

    [Fact]
    public async Task Deleting_a_non_existent_event_is_a_silent_no_op()
    {
        await using var f = new SqliteDbContextFixture();
        var (service, notifier) = Build(f);

        await service.DeleteEventAsync(Guid.NewGuid());

        Assert.Empty(notifier.Changes);
    }

    [Fact]
    public async Task ExportIcs_writes_every_event_to_the_given_path()
    {
        await using var f = new SqliteDbContextFixture();
        var notifier = new RecordingDataChangeNotifier();
        var exporter = new NullExportService();
        var service = new CalendarService(new CalendarRepository(f.Context), exporter, notifier);
        f.Context.AddRange(TestData.AnEvent(), TestData.AnEvent());
        await f.Context.SaveChangesAsync();

        await service.ExportIcsAsync("/tmp/x.ics");

        Assert.Equal(1, exporter.Calls);
        Assert.Equal("/tmp/x.ics", exporter.LastPath);
    }
}
