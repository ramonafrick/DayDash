using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Contracts;

public interface IStudyPlannerService
{
    Task<IReadOnlyList<Exam>> GetExamsAsync(CancellationToken ct = default);
    Task<Exam?> GetExamAsync(Guid examId, CancellationToken ct = default);

    /// <summary>Persists a new exam (computing RecommendedMinutes + DailyMinutes) and returns its id.</summary>
    Task<Guid> CreateExamAsync(Exam exam, CancellationToken ct = default);
    Task UpdateExamAsync(Exam exam, CancellationToken ct = default);
    Task DeleteExamAsync(Guid examId, CancellationToken ct = default);

    Task SetGoalCheckedAsync(Guid goalId, bool isChecked, CancellationToken ct = default);

    /// <summary>Replaces the goal list of an exam (used by the Camera OCR flow).</summary>
    Task SaveLearningGoalsAsync(Guid examId, IReadOnlyList<LearningGoal> goals, CancellationToken ct = default);

    /// <summary>goalCount x the subject's stored minutes-per-goal (15 fallback).</summary>
    Task<int> CalculateRecommendedMinutesAsync(int goalCount, string subject, CancellationToken ct = default);

    int CalculateDailyMinutes(int totalMinutes, DateOnly examDate);

    /// <summary>Open exams with a computed daily split (DailyMinutes &gt; 0, ExamDate &gt;= today).</summary>
    Task<IReadOnlyList<Exam>> GetTodayStudyPlanAsync(CancellationToken ct = default);
}
