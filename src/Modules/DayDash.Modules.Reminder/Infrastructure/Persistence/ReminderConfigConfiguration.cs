using DayDash.Modules.Reminder.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DayDash.Modules.Reminder.Infrastructure.Persistence;

internal sealed class ReminderConfigConfiguration : IEntityTypeConfiguration<ReminderConfig>
{
    public void Configure(EntityTypeBuilder<ReminderConfig> builder)
    {
        builder.ToTable("ReminderConfigs");
        builder.HasKey(c => c.Id);

        builder.Property(c => c.DailyStudyReminderTime).IsRequired();
        builder.Property(c => c.EventReminderDaysBefore).IsRequired();
        builder.Property(c => c.IsEnabled).IsRequired();
    }
}
