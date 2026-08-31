using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Reminder.Infrastructure.Persistence;

/// <summary>The Reminder module's contribution to the shared EF Core model.</summary>
public sealed class ReminderModelConfiguration : IModelConfiguration
{
    public void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ReminderConfigConfiguration());
    }
}
