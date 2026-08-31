using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Services;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Tests.Infrastructure;

public sealed class FakeSubjectConfigService : ISubjectConfigService
{
    public List<SubjectConfig> Subjects { get; } =
    [
        new() { Id = Guid.NewGuid(), Name = "Mathematik", MinutesPerGoal = 15 },
        new() { Id = Guid.NewGuid(), Name = "Deutsch", MinutesPerGoal = 20 },
    ];

    public Task<IReadOnlyList<SubjectConfig>> GetAllAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<SubjectConfig>>(Subjects.ToList());

    public Task<int> GetMinutesPerGoalAsync(string subjectName, CancellationToken ct = default)
        => Task.FromResult(Subjects.FirstOrDefault(s => s.Name == subjectName)?.MinutesPerGoal ?? SubjectConfig.DefaultMinutesPerGoal);

    public Task SaveAsync(SubjectConfig config, CancellationToken ct = default)
    {
        Subjects.RemoveAll(s => s.Id == config.Id);
        Subjects.Add(config);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        Subjects.RemoveAll(s => s.Id == id);
        return Task.CompletedTask;
    }
}

public sealed class FakeStudyPlannerService(FakeSubjectConfigService subjects) : IStudyPlannerService
{
    public List<Exam> Exams { get; } = [];
    public Exam? LastCreated { get; private set; }
    public Guid? LastGoalToggled { get; private set; }
    public Guid? LastSavedGoalsExamId { get; private set; }
    public IReadOnlyList<LearningGoal>? LastSavedGoals { get; private set; }

    public Task<IReadOnlyList<Exam>> GetExamsAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Exam>>(Exams.ToList());

    public Task<Exam?> GetExamAsync(Guid examId, CancellationToken ct = default)
        => Task.FromResult(Exams.FirstOrDefault(e => e.Id == examId));

    public Task<Guid> CreateExamAsync(Exam exam, CancellationToken ct = default)
    {
        if (exam.Id == Guid.Empty)
        {
            exam.Id = Guid.NewGuid();
        }

        LastCreated = exam;
        Exams.Add(exam);
        return Task.FromResult(exam.Id);
    }

    public Task UpdateExamAsync(Exam exam, CancellationToken ct = default) => Task.CompletedTask;

    public Task DeleteExamAsync(Guid examId, CancellationToken ct = default)
    {
        Exams.RemoveAll(e => e.Id == examId);
        return Task.CompletedTask;
    }

    public Task SetGoalCheckedAsync(Guid goalId, bool isChecked, CancellationToken ct = default)
    {
        LastGoalToggled = goalId;
        return Task.CompletedTask;
    }

    public Task SaveLearningGoalsAsync(Guid examId, IReadOnlyList<LearningGoal> goals, CancellationToken ct = default)
    {
        LastSavedGoalsExamId = examId;
        LastSavedGoals = goals.ToList();
        return Task.CompletedTask;
    }

    public async Task<int> CalculateRecommendedMinutesAsync(int goalCount, string subject, CancellationToken ct = default)
        => StudyMath.RecommendedMinutes(goalCount, await subjects.GetMinutesPerGoalAsync(subject, ct));

    public int CalculateDailyMinutes(int totalMinutes, DateOnly examDate)
        => StudyMath.DailyMinutes(totalMinutes, examDate, new DateOnly(2026, 3, 10));

    public Task<IReadOnlyList<Exam>> GetTodayStudyPlanAsync(CancellationToken ct = default)
        => Task.FromResult<IReadOnlyList<Exam>>(Exams.Where(e => e.DailyMinutes > 0).ToList());
}
