using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.StudyPlanner.Infrastructure.Persistence;

/// <summary>The StudyPlanner module's contribution to the shared EF Core model.</summary>
public sealed class StudyPlannerModelConfiguration : IModelConfiguration
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new SubjectConfigConfiguration());
        modelBuilder.ApplyConfiguration(new ExamConfiguration());
        modelBuilder.ApplyConfiguration(new LearningGoalConfiguration());
    }
}
