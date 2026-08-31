using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Calendar.Infrastructure;

/// <summary>
/// When an exam is deleted, clear the <see cref="CalendarEvent.LinkedExamId"/> on any event
/// that pointed at it (the event itself stays). Keeps the two modules interface-only coupled.
/// </summary>
public sealed class CalendarExamLinkHandler(DayDashDbContext context) : IDataChangeHandler
{
    public async Task HandleAsync(DataChange change, CancellationToken ct = default)
    {
        if (change.Kind != DataChangeKind.ExamDeleted)
        {
            return;
        }

        var linked = await context.Set<CalendarEvent>()
            .Where(e => e.LinkedExamId == change.EntityId)
            .ToListAsync(ct);

        if (linked.Count == 0)
        {
            return;
        }

        foreach (var e in linked)
        {
            e.LinkedExamId = null;
        }

        await context.SaveChangesAsync(ct);
    }
}
