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
        // No HasDefaultValue: the CLR property default (15) covers new rows, and a store default
        // would make an explicit MinutesPerGoal of 0 silently become 15 on INSERT only.
        builder.Property(s => s.MinutesPerGoal).IsRequired();
    }
}
