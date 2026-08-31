using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.StudyPlanner.Application.Services;

public class StudyPlannerService(
    IExamRepository examRepository,
    ISubjectConfigService subjects,
    IDataChangeNotifier notifier,
    TimeProvider timeProvider) : IStudyPlannerService
{
    private DateOnly Today => DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);

    public Task<IReadOnlyList<Exam>> GetExamsAsync(CancellationToken ct = default)
        => examRepository.ListAsync(ct);

    public Task<Exam?> GetExamAsync(Guid examId, CancellationToken ct = default)
        => examRepository.GetWithGoalsAsync(examId, ct);

    public async Task<int> CalculateRecommendedMinutesAsync(int goalCount, string subject, CancellationToken ct = default)
    {
        var minutesPerGoal = await subjects.GetMinutesPerGoalAsync(subject, ct);
        return StudyMath.RecommendedMinutes(goalCount, minutesPerGoal);
    }

    public int CalculateDailyMinutes(int totalMinutes, DateOnly examDate)
        => StudyMath.DailyMinutes(totalMinutes, examDate, Today);

    public async Task<Guid> CreateExamAsync(Exam exam, CancellationToken ct = default)
    {
        if (exam.Id == Guid.Empty)
        {
            exam.Id = Guid.NewGuid();
        }

        await ApplyComputedFieldsAsync(exam, ct);
        await examRepository.AddAsync(exam, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.ExamSaved, exam.Id), ct);
        return exam.Id;
    }

    public async Task UpdateExamAsync(Exam exam, CancellationToken ct = default)
    {
        await ApplyComputedFieldsAsync(exam, ct);
        await examRepository.UpdateAsync(exam, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.ExamSaved, exam.Id), ct);
    }

    public async Task DeleteExamAsync(Guid examId, CancellationToken ct = default)
    {
        var exam = await examRepository.GetByIdAsync(examId, ct);
        if (exam is null)
        {
            return;
        }

        await examRepository.DeleteAsync(exam, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.ExamDeleted, examId), ct);
    }

    public async Task SetGoalCheckedAsync(Guid goalId, bool isChecked, CancellationToken ct = default)
    {
        var goal = await examRepository.Context.Set<LearningGoal>().FirstOrDefaultAsync(g => g.Id == goalId, ct);
        if (goal is null || goal.IsChecked == isChecked)
        {
            return;
        }

        goal.IsChecked = isChecked;
        await examRepository.Context.SaveChangesAsync(ct);
    }

    public async Task SaveLearningGoalsAsync(Guid examId, IReadOnlyList<LearningGoal> goals, CancellationToken ct = default)
    {
        var ctx = examRepository.Context;
        var exam = await ctx.Set<Exam>().FirstOrDefaultAsync(e => e.Id == examId, ct);
        if (exam is null)
        {
            return;
        }

        var existing = await ctx.Set<LearningGoal>().Where(g => g.ExamId == examId).ToListAsync(ct);
        ctx.Set<LearningGoal>().RemoveRange(existing);

        var replacement = goals
            .Where(g => !string.IsNullOrWhiteSpace(g.Text))
            .Select((g, i) => new LearningGoal
            {
                Id = g.Id == Guid.Empty ? Guid.NewGuid() : g.Id,
                ExamId = examId,
                Text = g.Text.Trim(),
                IsChecked = g.IsChecked,
                SortOrder = i,
            })
            .ToList();
        ctx.Set<LearningGoal>().AddRange(replacement);

        exam.RecommendedMinutes = await CalculateRecommendedMinutesAsync(replacement.Count, exam.Subject, ct);
        await ctx.SaveChangesAsync(ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.ExamSaved, examId), ct);
    }

    public async Task<IReadOnlyList<Exam>> GetTodayStudyPlanAsync(CancellationToken ct = default)
    {
        var today = Today;
        var exams = await examRepository.ListAsync(ct);
        return exams.Where(e => e.DailyMinutes > 0 && e.ExamDate >= today).ToList();
    }

    private async Task ApplyComputedFieldsAsync(Exam exam, CancellationToken ct)
    {
        var goalCount = exam.LearningGoals.Count;
        exam.RecommendedMinutes = await CalculateRecommendedMinutesAsync(goalCount, exam.Subject, ct);
        exam.DailyMinutes = StudyMath.DailyMinutes(exam.TotalStudyMinutes, exam.ExamDate, Today);

        for (var i = 0; i < exam.LearningGoals.Count; i++)
        {
            exam.LearningGoals[i].SortOrder = i;
            exam.LearningGoals[i].ExamId = exam.Id;
        }
    }
}
