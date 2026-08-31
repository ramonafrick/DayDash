using DayDash.Modules.Calendar.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DayDash.Modules.Calendar.Infrastructure.Persistence;

internal sealed class EventTypeConfigConfiguration : IEntityTypeConfiguration<EventTypeConfig>
{
    public void Configure(EntityTypeBuilder<EventTypeConfig> builder)
    {
        builder.ToTable("EventTypeConfigs");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Key).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Color).IsRequired().HasMaxLength(20);

        // The built-in types have a unique key; user-created types use an empty key.
        builder.HasIndex(e => e.Key).IsUnique().HasFilter("\"Key\" <> ''");
    }
}
