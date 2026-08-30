using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore.Design;

namespace DayDash.Migrations;

/// <summary>
/// Used by <c>dotnet ef</c> (via <c>src/DayDash.Migrations</c>) to instantiate the context
/// with the complete model. Points at a throwaway file next to the project.
/// </summary>
public sealed class DayDashDesignTimeDbContextFactory : IDesignTimeDbContextFactory<DayDashDbContext>
{
    public DayDashDbContext CreateDbContext(string[] args)
    {
        var path = Path.Combine(AppContext.BaseDirectory, "daydash-design.db");
        return DayDashDbContextFactory.CreateSqlite(path);
    }
}
