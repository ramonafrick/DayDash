using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.StudyPlanner.Application.Services;

public sealed class SubjectConfigService(
    ISubjectConfigRepository repository,
    IDataChangeNotifier notifier) : ISubjectConfigService
{
    public Task<IReadOnlyList<SubjectConfig>> GetAllAsync(CancellationToken ct = default)
        => repository.GetAllAsync(ct);

    public async Task<int> GetMinutesPerGoalAsync(string subjectName, CancellationToken ct = default)
    {
        var config = await repository.GetByNameAsync(subjectName, ct);
        return config?.MinutesPerGoal ?? SubjectConfig.DefaultMinutesPerGoal;
    }

    public async Task SaveAsync(SubjectConfig config, CancellationToken ct = default)
    {
        await repository.UpsertAsync(config, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.SubjectConfigChanged, config.Id), ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
        await repository.DeleteAsync(id, ct);
        await notifier.NotifyAsync(new DataChange(DataChangeKind.SubjectConfigChanged, id), ct);
    }
}
