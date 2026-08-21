namespace DayDash.Modules.StudyPlanner.Domain;

public class SubjectConfig
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int MinutesPerGoal { get; set; } = 15;

    public static List<SubjectConfig> DefaultSubjects => new()
    {
        new SubjectConfig { Id = Guid.NewGuid(), Name = "Mathematik" },
        new SubjectConfig { Id = Guid.NewGuid(), Name = "Deutsch" },
        new SubjectConfig { Id = Guid.NewGuid(), Name = "NMG" },
        new SubjectConfig { Id = Guid.NewGuid(), Name = "Englisch" },
        new SubjectConfig { Id = Guid.NewGuid(), Name = "Französisch" }
    };
}