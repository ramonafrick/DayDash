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
        => examRepository.ReadOneAsync(examId, ct);

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
        var ctx = examRepository.Context;
        var tracked = await examRepository.GetWithGoalsAsync(exam.Id, ct);
        if (tracked is null)
        {
            return;
        }

        tracked.Title = exam.Title;
        tracked.Subject = exam.Subject;
        tracked.ExamDate = exam.ExamDate;
        tracked.TotalStudyMinutes = exam.TotalStudyMinutes;

        // Reconcile the goal list against the DbSet directly: drop removed, update matching by id, add new.
        var goalSet = ctx.Set<LearningGoal>();
        var incoming = exam.LearningGoals.Where(g => !string.IsNullOrWhiteSpace(g.Text)).ToList();
        var incomingIds = incoming.Where(g => g.Id != Guid.Empty).Select(g => g.Id).ToHashSet();
        var current = tracked.LearningGoals.ToList();

        foreach (var orphan in current.Where(g => !incomingIds.Contains(g.Id)))
        {
            goalSet.Remove(orphan);
        }

        var goalCount = 0;
        for (var i = 0; i < incoming.Count; i++)
        {
            var src = incoming[i];
            var existing = src.Id != Guid.Empty ? current.FirstOrDefault(g => g.Id == src.Id) : null;
            if (existing is null)
            {
                goalSet.Add(new LearningGoal
                {
                    Id = Guid.NewGuid(), ExamId = tracked.Id, Text = src.Text.Trim(), IsChecked = src.IsChecked, SortOrder = i,
                });
            }
            else
            {
                existing.Text = src.Text.Trim();
                existing.IsChecked = src.IsChecked;
                existing.SortOrder = i;
            }

            goalCount++;
        }

        tracked.RecommendedMinutes = await CalculateRecommendedMinutesAsync(goalCount, tracked.Subject, ct);
        tracked.DailyMinutes = StudyMath.DailyMinutes(tracked.TotalStudyMinutes, tracked.ExamDate, Today);

        await ctx.SaveChangesAsync(ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.ExamSaved, tracked.Id), ct);
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
                Id = Guid.NewGuid(), // full replacement - never reuse an incoming id (avoids a clash with the just-Deleted rows)
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
