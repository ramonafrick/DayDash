using System;
using System.Collections.Generic;
using System.Linq;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.Camera.Application.Services;

public class LearningGoalParser : ILearningGoalParser
{
    public List<LearningGoal> ParseToLearningGoals(string ocrText, Guid examId)
    {
        return ocrText
            .Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
            .Select((line, index) => new LearningGoal
            {
                ExamId = examId,
                Text = line.Trim(),
                SortOrder = index,
                IsChecked = false
            })
            .ToList();
    }
}