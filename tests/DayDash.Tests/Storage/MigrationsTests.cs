using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Xunit;

namespace DayDash.Tests.Storage;

public class MigrationsTests
{
    [Fact]
    public async Task Migrate_succeeds_on_a_fresh_database()
    {
        await using var fixture = new SqliteDbContextFixture();

        var applied = await fixture.Context.Database.GetAppliedMigrationsAsync();
        var pending = await fixture.Context.Database.GetPendingMigrationsAsync();

        Assert.Contains(applied, m => m.EndsWith("InitialCreate", StringComparison.Ordinal));
        Assert.Empty(pending);
    }

    [Fact]
    public async Task No_model_changes_are_left_unmigrated()
    {
        // Catches a committed entity edit without a matching migration.
        await using var fixture = new SqliteDbContextFixture();

        Assert.False(fixture.Context.Database.HasPendingModelChanges());
    }

    [Fact]
    public async Task All_six_tables_exist_after_migration()
    {
        await using var fixture = new SqliteDbContextFixture();
        var names = await fixture.Context.Database
            .SqlQueryRaw<string>("SELECT name FROM sqlite_master WHERE type = 'table'")
            .ToListAsync();

        foreach (var table in new[]
                 {
                     "CalendarEvents", "EventTypeConfigs", "Exams", "LearningGoals",
                     "SubjectConfigs", "ReminderConfigs",
                 })
        {
            Assert.Contains(table, names);
        }
    }
}
