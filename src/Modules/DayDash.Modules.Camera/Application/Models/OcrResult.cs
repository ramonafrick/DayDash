using System;
using System.Collections.Generic;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.Camera.Application.Models;

public class OcrResult
{
    public string RawText { get; set; } = string.Empty;
    public List<LearningGoal> ParsedGoals { get; set; } = new();
    public DateTime CapturedAt { get; set; } = DateTime.UtcNow;
}