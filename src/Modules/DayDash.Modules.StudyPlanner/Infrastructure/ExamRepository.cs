using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.StudyPlanner.Infrastructure;

public class ExamRepository(DayDashDbContext context, TimeProvider timeProvider)
    : BaseRepository<Exam>(context), IExamRepository
{
    public async Task<Exam?> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => await WithGoals().FirstOrDefaultAsync(e => e.ExamDate == date, ct);

    public async Task<Exam?> GetWithGoalsAsync(Guid examId, CancellationToken ct = default)
        => await WithGoals().FirstOrDefaultAsync(e => e.Id == examId, ct);

    /// <summary>All exams whose date falls in the next <paramref name="days"/> days (a window, not a row count).</summary>
    public async Task<IReadOnlyList<Exam>> GetUpcomingAsync(int days, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var until = today.AddDays(days);
        return await WithGoals()
            .Where(e => e.ExamDate >= today && e.ExamDate <= until)
            .OrderBy(e => e.ExamDate)
            .ToListAsync(ct);
    }

    private IQueryable<Exam> WithGoals() =>
        _context.Set<Exam>().Include(e => e.LearningGoals.OrderBy(g => g.SortOrder));
}
