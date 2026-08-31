using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Contracts;

public interface ISubjectConfigRepository
{
    Task<IReadOnlyList<SubjectConfig>> GetAllAsync(CancellationToken ct = default);

    Task<SubjectConfig?> GetByNameAsync(string name, CancellationToken ct = default);

    Task UpsertAsync(SubjectConfig config, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
