using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Calendar.Infrastructure.Persistence;

/// <summary>The Calendar module's contribution to the shared EF Core model.</summary>
public sealed class CalendarModelConfiguration : IModelConfiguration
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new EventTypeConfigConfiguration());
        modelBuilder.ApplyConfiguration(new CalendarEventConfiguration());
    }
}
