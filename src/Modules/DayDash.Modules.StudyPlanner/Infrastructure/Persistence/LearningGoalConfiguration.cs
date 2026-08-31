using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DayDash.Modules.StudyPlanner.Infrastructure.Persistence;

internal sealed class LearningGoalConfiguration : IEntityTypeConfiguration<LearningGoal>
{
    public void Configure(EntityTypeBuilder<LearningGoal> builder)
    {
        builder.ToTable("LearningGoals");
        builder.HasKey(g => g.Id);

        builder.Property(g => g.Text).IsRequired().HasMaxLength(200);

        builder.HasIndex(g => new { g.ExamId, g.SortOrder });
    }
}
