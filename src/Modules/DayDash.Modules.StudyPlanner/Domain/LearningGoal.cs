namespace DayDash.Modules.StudyPlanner.Domain;

public class LearningGoal
{
    public Guid Id { get; set; }

    public Guid ExamId { get; set; }

    public Exam? Exam { get; set; }

    public string Text { get; set; } = string.Empty;

    public bool IsChecked { get; set; }

    public int SortOrder { get; set; }
}
