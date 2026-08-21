using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.Extensions.Logging;

namespace DayDash.Modules.StudyPlanner.Application.Services;

public class StudyPlannerService(IExamRepository examRepository, ILogger<StudyPlannerService> logger) : IStudyPlannerService
{
    private readonly IExamRepository _examRepository = examRepository;
    private readonly ILogger<StudyPlannerService> _logger = logger;

    public int CalculateRecommendedMinutes(int goalCount, string subject)
    {
        // Assuming SubjectConfig is fetched elsewhere
        var subjectConfig = SubjectConfig.DefaultSubjects.FirstOrDefault(s => s.Name == subject);
        return goalCount * (subjectConfig?.MinutesPerGoal ?? 15);
    }

    public int CalculateDailyMinutes(int totalMinutes, DateOnly examDate)
    {
        var daysUntilExam = Math.Max((examDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days, 1);
        return totalMinutes / daysUntilExam;
    }

    public async Task<IReadOnlyList<LearningGoal>> GetTodayStudyPlanAsync(CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var exams = await _examRepository.GetUpcomingAsync(1, ct);

        return exams
            .Where(e => e.DailyMinutes > 0 && e.ExamDate == today)
            .SelectMany(e => e.LearningGoals)
            .ToList();
    }

    public async Task<IReadOnlyList<SubjectConfig>> GetSubjectConfigsAsync(CancellationToken ct = default)
    {
        // Assuming fetching from DbContext
        return SubjectConfig.DefaultSubjects;
    }

    public async Task SaveSubjectConfigAsync(SubjectConfig config, CancellationToken ct = default)
    {
        // Save logic here
        _logger.LogInformation("SubjectConfig saved: {Config}", config);
    }

    public async Task CreateExamAsync(Exam exam, CancellationToken ct = default)
    {
        await _examRepository.AddAsync(exam, ct);
    }

    public async Task UpdateExamAsync(Exam exam, CancellationToken ct = default)
    {
        await _examRepository.UpdateAsync(exam, ct);
    }

    public async Task DeleteExamAsync(Guid examId, CancellationToken ct = default)
    {
        await _examRepository.DeleteAsync(examId, ct);
    }
}