using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.StudyPlanner.Infrastructure;

public sealed class SubjectConfigRepository(DayDashDbContext context) : ISubjectConfigRepository
{
    public async Task<IReadOnlyList<SubjectConfig>> GetAllAsync(CancellationToken ct = default)
        => await context.Set<SubjectConfig>().AsNoTracking().OrderBy(s => s.Name).ToListAsync(ct);

    public Task<SubjectConfig?> GetByNameAsync(string name, CancellationToken ct = default)
        => context.Set<SubjectConfig>().AsNoTracking().FirstOrDefaultAsync(s => s.Name == name, ct);

    public async Task UpsertAsync(SubjectConfig config, CancellationToken ct = default)
    {
        var existing = await context.Set<SubjectConfig>().FirstOrDefaultAsync(s => s.Id == config.Id, ct);
        if (existing is null)
        {
            context.Set<SubjectConfig>().Add(new SubjectConfig
            {
                Id = config.Id == Guid.Empty ? Guid.NewGuid() : config.Id,
                Name = config.Name,
                MinutesPerGoal = config.MinutesPerGoal,
            });
        }
        else
        {
            existing.Name = config.Name;
            existing.MinutesPerGoal = config.MinutesPerGoal;
        }

        await context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var existing = await context.Set<SubjectConfig>().FirstOrDefaultAsync(s => s.Id == id, ct);
        if (existing is not null)
        {
            context.Set<SubjectConfig>().Remove(existing);
            await context.SaveChangesAsync(ct);
        }
    }
}
