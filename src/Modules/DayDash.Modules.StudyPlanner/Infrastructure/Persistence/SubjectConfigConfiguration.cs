using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DayDash.Modules.StudyPlanner.Infrastructure.Persistence;

internal sealed class SubjectConfigConfiguration : IEntityTypeConfiguration<SubjectConfig>
{
    public void Configure(EntityTypeBuilder<SubjectConfig> builder)
    {
        builder.ToTable("SubjectConfigs");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name).IsRequired().HasMaxLength(200);
        builder.Property(s => s.MinutesPerGoal).HasDefaultValue(SubjectConfig.DefaultMinutesPerGoal);
    }
}
