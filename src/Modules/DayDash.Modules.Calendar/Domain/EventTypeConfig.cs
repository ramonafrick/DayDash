namespace DayDash.Modules.Calendar.Domain;

public class EventTypeConfig
{
    public Guid Id { get; set; }

    /// <summary>
    /// Stable, non-localized identifier for the six built-in types (empty for user-created types).
    /// All logic branches on this; only <see cref="Name"/> is localized / renameable.
    /// </summary>
    public string Key { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Color { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    /// <summary>The <see cref="Key"/> of the "exam" type - the one that triggers the exam assistant.</summary>
    public const string ExamKey = "exam";

    /// <summary>
    /// The six built-in event types with fixed ids and colours (Requirements.md §5.1).
    /// Names are resolved from <c>CalendarResources</c> by the seeder, so they stay localizable
    /// and renameable as user data afterwards.
    /// </summary>
    public static readonly IReadOnlyList<DefaultEventType> Defaults =
    [
        new(new Guid("6f1b0e00-0000-4000-a000-000000000001"), ExamKey,      "#E53935"),
        new(new Guid("6f1b0e00-0000-4000-a000-000000000002"), "homework",    "#43A047"),
        new(new Guid("6f1b0e00-0000-4000-a000-000000000003"), "holidays",    "#1E88E5"),
        new(new Guid("6f1b0e00-0000-4000-a000-000000000004"), "birthday",    "#FDD835"),
        new(new Guid("6f1b0e00-0000-4000-a000-000000000005"), "appointment", "#8E24AA"),
        new(new Guid("6f1b0e00-0000-4000-a000-000000000006"), "other",       "#00ACC1"),
    ];
}

/// <summary>A built-in event type before its localized name is applied.</summary>
public readonly record struct DefaultEventType(Guid Id, string Key, string Color)
{
    /// <summary>Resource key for the localized display name, e.g. "DefaultEventType_Exam".</summary>
    public string ResourceKey => $"DefaultEventType_{char.ToUpperInvariant(Key[0])}{Key[1..]}";
}
