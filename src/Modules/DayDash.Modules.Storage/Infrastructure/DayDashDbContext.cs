using Microsoft.EntityFrameworkCore;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.Storage.Infrastructure;

public class DayDashDbContext : DbContext
{
    private readonly string _dbPath;

    public DayDashDbContext(string dbPath)
    {
        _dbPath = dbPath;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={_dbPath}");
    }

    public DbSet<CalendarEvent> CalendarEvents { get; set; } = default!;
    public DbSet<EventTypeConfig> EventTypeConfigs { get; set; } = default!;
    public DbSet<Exam> Exams { get; set; } = default!;
    public DbSet<LearningGoal> LearningGoals { get; set; } = default!;
    public DbSet<SubjectConfig> SubjectConfigs { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CalendarEvent>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.EventType).IsRequired();
            entity.Property(e => e.Date).IsRequired();
        });

        modelBuilder.Entity<EventTypeConfig>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Color).IsRequired();
        });

        modelBuilder.Entity<Exam>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired();
            entity.Property(e => e.Subject).IsRequired();
            entity.Property(e => e.ExamDate).IsRequired();
            entity.Property(e => e.TotalStudyMinutes).IsRequired();
            entity.Property(e => e.RecommendedMinutes).IsRequired();
            entity.Property(e => e.DailyMinutes).IsRequired();
        });

        modelBuilder.Entity<LearningGoal>(entity =>
        {
            entity.HasKey(lg => lg.Id);
            entity.Property(lg => lg.Text).IsRequired();
            entity.Property(lg => lg.IsChecked).IsRequired();
            entity.Property(lg => lg.SortOrder).IsRequired();
        });

        modelBuilder.Entity<SubjectConfig>(entity =>
        {
            entity.HasKey(sc => sc.Id);
            entity.Property(sc => sc.Name).IsRequired();
            entity.Property(sc => sc.MinutesPerGoal).HasDefaultValue(15);
        });
    }
}