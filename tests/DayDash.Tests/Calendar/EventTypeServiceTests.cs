using DayDash.Modules.Calendar.Application.Services;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Infrastructure;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.Calendar;

public class EventTypeServiceTests
{
    private static CalendarService Build(SqliteDbContextFixture f)
        => new(new CalendarRepository(f.Context), new NullExportService(), new RecordingDataChangeNotifier());

    [Fact]
    public async Task Deleting_a_type_used_by_events_keeps_the_events_and_nulls_their_link()
    {
        await using var f = new SqliteDbContextFixture();
        var service = Build(f);
        var type = new EventTypeConfig { Id = Guid.NewGuid(), Key = "", Name = "Sport", Color = "#0f0" };
        f.Context.Add(type);
        for (var i = 0; i < 3; i++)
        {
            var e = TestData.AnEvent();
            e.EventTypeId = type.Id;
            f.Context.Add(e);
        }

        await f.Context.SaveChangesAsync();
        f.Context.ChangeTracker.Clear();

        await service.DeleteEventTypeAsync(type.Id);

        Assert.Equal(3, await f.Context.Set<CalendarEvent>().CountAsync());
        Assert.All(await f.Context.Set<CalendarEvent>().ToListAsync(), e => Assert.Null(e.EventTypeId));
        Assert.Equal(0, await f.Context.Set<EventTypeConfig>().CountAsync());
    }

    [Fact]
    public async Task Renaming_a_default_type_keeps_its_Key_and_IsDefault_flag()
    {
        await using var f = new SqliteDbContextFixture();
        await SeedingHost.Initializer(f.Context).InitializeAsync();
        var service = Build(f);
        var exam = await f.Context.Set<EventTypeConfig>().SingleAsync(t => t.Key == EventTypeConfig.ExamKey);

        await service.SaveEventTypeAsync(new EventTypeConfig
        {
            Id = exam.Id,
            Key = "tampered",
            Name = "Klausur",
            Color = "#123456",
            IsDefault = false,
        });
        f.Context.ChangeTracker.Clear();

        var reloaded = await f.Context.Set<EventTypeConfig>().SingleAsync(t => t.Id == exam.Id);
        Assert.Equal("Klausur", reloaded.Name);
        Assert.Equal("#123456", reloaded.Color);
        Assert.Equal(EventTypeConfig.ExamKey, reloaded.Key);
        Assert.True(reloaded.IsDefault);
    }

    [Fact]
    public async Task Adding_a_new_type_persists_it_with_an_empty_key()
    {
        await using var f = new SqliteDbContextFixture();
        var service = Build(f);

        await service.SaveEventTypeAsync(new EventTypeConfig
        {
            Id = Guid.NewGuid(), Key = "", Name = "Vereinstraining", Color = "#abcdef", IsDefault = false,
        });

        var added = await f.Context.Set<EventTypeConfig>().SingleAsync();
        Assert.Equal("Vereinstraining", added.Name);
        Assert.Equal(string.Empty, added.Key);
        Assert.False(added.IsDefault);
    }
}
