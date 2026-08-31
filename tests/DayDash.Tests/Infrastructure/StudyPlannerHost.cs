using DayDash.Modules.StudyPlanner.Application.Contracts;
using DayDash.Modules.StudyPlanner.Application.Services;
using DayDash.Modules.StudyPlanner.Infrastructure;

namespace DayDash.Tests.Infrastructure;

/// <summary>Wires the StudyPlanner service graph against a SQLite fixture for service tests.</summary>
public sealed class StudyPlannerHost
{
    public StudyPlannerHost(SqliteDbContextFixture fixture)
    {
        Notifier = new RecordingDataChangeNotifier();
        SubjectRepository = new SubjectConfigRepository(fixture.Context);
        ExamRepository = new ExamRepository(fixture.Context, fixture.Time);
        SubjectService = new SubjectConfigService(SubjectRepository, Notifier);
        Service = new StudyPlannerService(ExamRepository, SubjectService, Notifier, fixture.Time);
    }

    public RecordingDataChangeNotifier Notifier { get; }
    public ISubjectConfigRepository SubjectRepository { get; }
    public IExamRepository ExamRepository { get; }
    public ISubjectConfigService SubjectService { get; }
    public IStudyPlannerService Service { get; }
}
