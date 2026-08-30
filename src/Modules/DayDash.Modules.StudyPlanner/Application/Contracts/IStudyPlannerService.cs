using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Contracts;

public interface IStudyPlannerService
{
    Task CreateExamAsync(Exam exam, CancellationToken ct = default);
    Task UpdateExamAsync(Exam exam, CancellationToken ct = default);
    Task DeleteExamAsync(Guid examId, CancellationToken ct = default);

    int CalculateRecommendedMinutes(int goalCount, string subject);
    int CalculateDailyMinutes(int totalMinutes, DateOnly examDate);

    Task<IReadOnlyList<Exam>> GetTodayStudyPlanAsync(CancellationToken ct = default);
    Task<IReadOnlyList<SubjectConfig>> GetSubjectConfigsAsync(CancellationToken ct = default);
    Task SaveSubjectConfigAsync(SubjectConfig config, CancellationToken ct = default);
}