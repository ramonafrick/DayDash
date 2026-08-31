using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.StudyPlanner.Infrastructure;

public class ExamRepository(DayDashDbContext context, TimeProvider timeProvider)
    : BaseRepository<Exam>(context), IExamRepository
{
    public async Task<IReadOnlyList<Exam>> ListAsync(CancellationToken ct = default)
        => await ReadGoals().OrderBy(e => e.ExamDate).ThenBy(e => e.Title).ToListAsync(ct);

    public async Task<Exam?> GetByDateAsync(DateOnly date, CancellationToken ct = default)
        => await ReadGoals().FirstOrDefaultAsync(e => e.ExamDate == date, ct);

    public async Task<IReadOnlyList<Exam>> GetUpcomingAsync(int days, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(timeProvider.GetLocalNow().Date);
        var until = today.AddDays(days);
        return await ReadGoals()
            .Where(e => e.ExamDate >= today && e.ExamDate <= until)
            .OrderBy(e => e.ExamDate)
            .ToListAsync(ct);
    }

    public async Task<Exam?> ReadOneAsync(Guid examId, CancellationToken ct = default)
        => await ReadGoals().FirstOrDefaultAsync(e => e.Id == examId, ct);

    public async Task<Exam?> GetWithGoalsAsync(Guid examId, CancellationToken ct = default)
        => await _context.Set<Exam>()
            .Include(e => e.LearningGoals.OrderBy(g => g.SortOrder))
            .FirstOrDefaultAsync(e => e.Id == examId, ct);

    private IQueryable<Exam> ReadGoals() =>
        _context.Set<Exam>().AsNoTracking().Include(e => e.LearningGoals.OrderBy(g => g.SortOrder));
}
