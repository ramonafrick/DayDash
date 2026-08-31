using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Contracts;

public interface IExamRepository : IRepository<Exam>
{
    /// <summary>All exams ordered by date, goals included, read-only (no change tracking).</summary>
    Task<IReadOnlyList<Exam>> ListAsync(CancellationToken ct = default);

    Task<Exam?> GetByDateAsync(DateOnly date, CancellationToken ct = default);

    /// <summary>Exams whose date is within the next <paramref name="days"/> days (a window, not a row count).</summary>
    Task<IReadOnlyList<Exam>> GetUpcomingAsync(int days, CancellationToken ct = default);

    /// <summary>The exam with its goals, tracked (for edit / goal toggling).</summary>
    Task<Exam?> GetWithGoalsAsync(Guid examId, CancellationToken ct = default);
}
