using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Services;

public class StudyPlannerService(
    IExamRepository examRepository,
    ISubjectConfigRepository subjectRepository,
    TimeProvider timeProvider) : IStudyPlannerService
{
    // Per-subject MinutesPerGoal is applied in Slice 3 (needs an async lookup); for now every
    // subject uses the default rate.
    public int CalculateRecommendedMinutes(int goalCount, string subject)
        => goalCount * SubjectConfig.DefaultMinutesPerGoal;

    public int CalculateDailyMinutes(int totalMinutes, DateOnly examDate)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var daysUntilExam = Math.Max(examDate.DayNumber - today.DayNumber, 1);
        return totalMinutes / daysUntilExam;
    }

    public async Task<IReadOnlyList<Exam>> GetTodayStudyPlanAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var exams = await examRepository.GetUpcomingAsync(30, ct);
        return exams.Where(e => e.DailyMinutes > 0 && e.ExamDate >= today).ToList();
    }

    public async Task<IReadOnlyList<SubjectConfig>> GetSubjectConfigsAsync(CancellationToken ct = default)
        => await subjectRepository.GetAllAsync(ct);

    public Task SaveSubjectConfigAsync(SubjectConfig config, CancellationToken ct = default)
        => subjectRepository.UpsertAsync(config, ct);

    public Task CreateExamAsync(Exam exam, CancellationToken ct = default)
        => examRepository.AddAsync(exam, ct);

    public Task UpdateExamAsync(Exam exam, CancellationToken ct = default)
        => examRepository.UpdateAsync(exam, ct);

    public async Task DeleteExamAsync(Guid examId, CancellationToken ct = default)
    {
        var exam = await examRepository.GetByIdAsync(examId, ct);
        if (exam is not null)
        {
            await examRepository.DeleteAsync(exam, ct);
        }
    }
}
