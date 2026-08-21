using System;
using System.Collections.Generic;

namespace DayDash.Modules.Calendar.Domain;

public class EventTypeConfig
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public static IReadOnlyList<EventTypeConfig> DefaultEventTypes => new List<EventTypeConfig>
    {
        new() { Id = Guid.NewGuid(), Name = "Prüfung", Color = "#FF0000", IsDefault = true },
        new() { Id = Guid.NewGuid(), Name = "Hausaufgaben", Color = "#00FF00", IsDefault = true },
        new() { Id = Guid.NewGuid(), Name = "Schulferien", Color = "#0000FF", IsDefault = true },
        new() { Id = Guid.NewGuid(), Name = "Geburtstag", Color = "#FFFF00", IsDefault = true },
        new() { Id = Guid.NewGuid(), Name = "Abmachung", Color = "#FF00FF", IsDefault = true },
        new() { Id = Guid.NewGuid(), Name = "Sonstiges", Color = "#00FFFF", IsDefault = true }
    };
}