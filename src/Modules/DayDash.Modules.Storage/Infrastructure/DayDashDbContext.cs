using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Storage.Infrastructure;

/// <summary>
/// Shared EF Core context for all modules. It is deliberately entity-agnostic so the
/// Storage module has no compile-time dependency on the feature modules (which would
/// otherwise create a circular reference). Entity types are discovered at runtime via
/// <see cref="DbContext.Set{TEntity}()"/> and their navigation properties.
/// </summary>
public class DayDashDbContext : DbContext
{
    public DayDashDbContext(DbContextOptions<DayDashDbContext> options)
        : base(options)
    {
    }
}
