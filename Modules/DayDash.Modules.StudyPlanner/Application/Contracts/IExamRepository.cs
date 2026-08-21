using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Contracts;

public interface IExamRepository : IRepository<Exam>
{
    Task<Exam?> GetByDateAsync(DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<Exam>> GetUpcomingAsync(int days, CancellationToken ct = default);
    Task<Exam?> GetWithGoalsAsync(Guid examId, CancellationToken ct = default);
}