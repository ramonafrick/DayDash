using DayDash.Migrations;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Tests.Infrastructure;

/// <summary>
/// EF Core InMemory provider - mirrors the <c>DayDash.Web</c> preview host (FR-S6).
/// Use <see cref="SqliteDbContextFixture"/> for anything that depends on real relational
/// semantics; use this only to prove the InMemory path works.
/// </summary>
public sealed class InMemoryDbContextFixture : IAsyncDisposable
{
    public InMemoryDbContextFixture()
    {
        var options = new DbContextOptionsBuilder<DayDashDbContext>()
            .UseInMemoryDatabase($"DayDashTests-{Guid.NewGuid()}")
            .Options;

        Context = new DayDashDbContext(options, DayDashModelConfigurations.All);
        Context.Database.EnsureCreated();
    }

    public DayDashDbContext Context { get; }

    public ValueTask DisposeAsync() => Context.DisposeAsync();
}
