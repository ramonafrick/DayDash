using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Contracts;

public interface ISubjectConfigService
{
    Task<IReadOnlyList<SubjectConfig>> GetAllAsync(CancellationToken ct = default);

    /// <summary>Stored minutes-per-goal for the subject, or the default (15) when unknown/deleted.</summary>
    Task<int> GetMinutesPerGoalAsync(string subjectName, CancellationToken ct = default);

    Task SaveAsync(SubjectConfig config, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
}
