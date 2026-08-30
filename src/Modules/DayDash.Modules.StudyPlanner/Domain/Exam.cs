namespace DayDash.Modules.StudyPlanner.Domain;

public class Exam
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public DateOnly ExamDate { get; set; }
    public int TotalStudyMinutes { get; set; }
    public int RecommendedMinutes { get; set; }
    public int DailyMinutes { get; set; }

    public List<LearningGoal> LearningGoals { get; set; } = new();
}