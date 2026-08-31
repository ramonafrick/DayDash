namespace DayDash.Modules.StudyPlanner.Domain;

public class SubjectConfig
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int MinutesPerGoal { get; set; } = 15;

    public const int DefaultMinutesPerGoal = 15;

    /// <summary>
    /// The five built-in subjects with fixed ids (Requirements.md §5.3). Names are resolved from
    /// <c>StudyPlannerResources</c> by the seeder, so they stay localizable and renameable.
    /// </summary>
    public static readonly IReadOnlyList<DefaultSubject> Defaults =
    [
        new(new Guid("5c2a0e00-0000-4000-a000-000000000001"), "DefaultSubject_Math"),
        new(new Guid("5c2a0e00-0000-4000-a000-000000000002"), "DefaultSubject_German"),
        new(new Guid("5c2a0e00-0000-4000-a000-000000000003"), "DefaultSubject_Nmg"),
        new(new Guid("5c2a0e00-0000-4000-a000-000000000004"), "DefaultSubject_English"),
        new(new Guid("5c2a0e00-0000-4000-a000-000000000005"), "DefaultSubject_French"),
    ];
}

/// <summary>A built-in subject before its localized name is applied.</summary>
public readonly record struct DefaultSubject(Guid Id, string ResourceKey);
