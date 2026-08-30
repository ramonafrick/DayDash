using DayDash.Modules.Storage.Application.Contracts;

namespace DayDash.Migrations;

/// <summary>
/// The full set of <see cref="IModelConfiguration"/> contributions, in a fixed order, used by
/// the design-time / widget construction paths where DI is not available. The DI path
/// (app hosts, tests) enumerates the same implementations from the container instead.
/// Feature modules add their entries here in Slice 1.
/// </summary>
public static class DayDashModelConfigurations
{
    public static IReadOnlyList<IModelConfiguration> All { get; } = [];
}
