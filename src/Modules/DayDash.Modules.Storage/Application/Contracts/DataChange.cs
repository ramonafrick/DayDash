namespace DayDash.Modules.Storage.Application.Contracts;

public enum DataChangeKind
{
    CalendarEventSaved,
    CalendarEventDeleted,
    ExamSaved,
    ExamDeleted,
    EventTypeChanged,
    SubjectConfigChanged,
    ReminderConfigChanged,
}

/// <summary>A single persisted change, published after a write so cross-cutting concerns
/// (reminder rescheduling, widget refresh, exam/event unlinking) can react without the
/// writing module referencing them.</summary>
public readonly record struct DataChange(DataChangeKind Kind, Guid EntityId);

/// <summary>Something that reacts to a <see cref="DataChange"/>. Registered by the module or
/// host that owns the reaction; a failing handler is logged and swallowed by the notifier.</summary>
public interface IDataChangeHandler
{
    Task HandleAsync(DataChange change, CancellationToken ct = default);
}

/// <summary>Fan-out point that feature services call after every write.</summary>
public interface IDataChangeNotifier
{
    Task NotifyAsync(DataChange change, CancellationToken ct = default);
}
