using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DayDash.Modules.StudyPlanner.Infrastructure.Persistence;

internal sealed class ExamConfiguration : IEntityTypeConfiguration<Exam>
{
    public void Configure(EntityTypeBuilder<Exam> builder)
    {
        builder.ToTable("Exams");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Subject).IsRequired().HasMaxLength(200);
        builder.Property(e => e.ExamDate).IsRequired();

        builder.HasIndex(e => e.ExamDate);

        builder.HasMany(e => e.LearningGoals)
            .WithOne(g => g.Exam!)
            .HasForeignKey(g => g.ExamId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
