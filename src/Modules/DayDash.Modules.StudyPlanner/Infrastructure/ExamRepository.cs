using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.StudyPlanner.Infrastructure;

public class ExamRepository(DayDashDbContext dbContext) : BaseRepository<Exam>(dbContext), IExamRepository
{
    public async Task<Exam?> GetByDateAsync(DateOnly date, CancellationToken ct = default)
    {
        return await _context.Set<Exam>()
            .Include(e => e.LearningGoals)
            .FirstOrDefaultAsync(e => e.ExamDate == date, ct);
    }

    public async Task<Exam?> GetWithGoalsAsync(Guid examId, CancellationToken ct = default)
    {
        return await _context.Set<Exam>()
            .Include(e => e.LearningGoals)
            .FirstOrDefaultAsync(e => e.Id == examId, ct);
    }

    public async Task<IReadOnlyList<Exam>> GetUpcomingAsync(int days, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        return await _context.Set<Exam>()
            .Include(e => e.LearningGoals)
            .Where(e => e.ExamDate >= today)
            .OrderBy(e => e.ExamDate)
            .Take(days)
            .ToListAsync(ct);
    }
}
