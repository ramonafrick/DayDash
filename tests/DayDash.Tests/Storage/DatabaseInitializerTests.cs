using System.Globalization;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Reminder.Domain;
using DayDash.Modules.StudyPlanner.Domain;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.Storage;

public class DatabaseInitializerTests : CultureIsolatedTest
{
    [Fact]
    public async Task Seeds_six_event_types_five_subjects_and_one_reminder_config()
    {
        await using var f = new SqliteDbContextFixture();

        await SeedingHost.Initializer(f.Context).InitializeAsync();

        Assert.Equal(6, await f.Context.Set<EventTypeConfig>().CountAsync());
        Assert.Equal(5, await f.Context.Set<SubjectConfig>().CountAsync());
        Assert.Equal(1, await f.Context.Set<ReminderConfig>().CountAsync());
    }

    [Fact]
    public async Task Seeded_event_types_use_localized_names_stable_ids_and_the_exam_key()
    {
        CultureInfo.CurrentUICulture = new CultureInfo("de-CH");
        await using var f = new SqliteDbContextFixture();
        await SeedingHost.Initializer(f.Context).InitializeAsync();

        var exam = await f.Context.Set<EventTypeConfig>().SingleAsync(t => t.Key == EventTypeConfig.ExamKey);
        Assert.Equal(EventTypeConfig.Defaults[0].Id, exam.Id);
        Assert.Equal("Prüfung", exam.Name);   // neutral resx is de-CH
        Assert.True(exam.IsDefault);
    }

    [Fact]
    public async Task Running_twice_does_not_duplicate_seed_data()
    {
        await using var f = new SqliteDbContextFixture();

        await SeedingHost.Initializer(f.Context).InitializeAsync();
        await SeedingHost.Initializer(f.Context).InitializeAsync();

        Assert.Equal(6, await f.Context.Set<EventTypeConfig>().CountAsync());
        Assert.Equal(5, await f.Context.Set<SubjectConfig>().CountAsync());
        Assert.Equal(1, await f.Context.Set<ReminderConfig>().CountAsync());
    }

    [Fact]
    public async Task A_partially_seeded_table_is_left_untouched()
    {
        await using var f = new SqliteDbContextFixture();
        f.Context.Set<SubjectConfig>().Add(new SubjectConfig { Id = Guid.NewGuid(), Name = "Custom", MinutesPerGoal = 30 });
        await f.Context.SaveChangesAsync();

        await SeedingHost.Initializer(f.Context).InitializeAsync();

        Assert.Equal(1, await f.Context.Set<SubjectConfig>().CountAsync());
        Assert.Equal("Custom", await f.Context.Set<SubjectConfig>().Select(s => s.Name).SingleAsync());
    }

    [Fact]
    public async Task SubjectConfig_MinutesPerGoal_defaults_to_15()
    {
        await using var f = new SqliteDbContextFixture();
        await SeedingHost.Initializer(f.Context).InitializeAsync();

        Assert.All(await f.Context.Set<SubjectConfig>().ToListAsync(),
            s => Assert.Equal(15, s.MinutesPerGoal));
    }

    [Fact]
    public async Task Seeds_identically_on_the_InMemory_provider()
    {
        await using var f = new InMemoryDbContextFixture();

        await SeedingHost.Initializer(f.Context).InitializeAsync();

        Assert.Equal(6, await f.Context.Set<EventTypeConfig>().CountAsync());
        Assert.Equal(5, await f.Context.Set<SubjectConfig>().CountAsync());
        Assert.Equal(1, await f.Context.Set<ReminderConfig>().CountAsync());
    }
}
