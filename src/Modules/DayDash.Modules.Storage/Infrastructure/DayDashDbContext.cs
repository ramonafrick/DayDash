using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Storage.Infrastructure;

/// <summary>
/// Shared EF Core context for all modules. It carries no <c>DbSet</c> properties: the model is
/// assembled from <see cref="IModelConfiguration"/> contributions supplied by the feature
/// modules, so the Storage module never references them (avoids a circular dependency —
/// see <c>docs/20260830_plan.md</c>, AD-1). Access entities via <see cref="DbContext.Set{TEntity}()"/>.
/// </summary>
public class DayDashDbContext(
    DbContextOptions<DayDashDbContext> options,
    IEnumerable<IModelConfiguration> modelConfigurations) : DbContext(options)
{
    private readonly IEnumerable<IModelConfiguration> _modelConfigurations = modelConfigurations;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var configuration in _modelConfigurations)
        {
            configuration.Apply(modelBuilder);
        }
    }
}
