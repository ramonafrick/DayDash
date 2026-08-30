using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace DayDash.Tests.Storage;

public class SqliteDbContextFixtureTests
{
    [Fact]
    public async Task Fixture_opens_migrates_and_disposes_cleanly()
    {
        await using var fixture = new SqliteDbContextFixture();

        Assert.True(fixture.Context.Database.IsRelational());
        Assert.False((await fixture.Context.Database.GetPendingMigrationsAsync()).Any());
    }

    [Fact]
    public async Task InMemory_fixture_creates_and_disposes_cleanly()
    {
        await using var fixture = new InMemoryDbContextFixture();

        Assert.False(fixture.Context.Database.IsRelational());
    }
}
