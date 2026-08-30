using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Storage.Application.Contracts;

/// <summary>
/// A feature module's contribution to the shared EF Core model. Each entity-owning module
/// ships exactly one implementation (in <c>Infrastructure/Persistence/</c>) that applies its
/// own <see cref="IEntityTypeConfiguration{TEntity}"/> classes. The Storage module has no
/// compile-time dependency on the feature modules; it enumerates these via DI instead
/// (see <c>docs/20260830_plan.md</c>, AD-1).
/// </summary>
public interface IModelConfiguration
{
    void Apply(ModelBuilder modelBuilder);
}
