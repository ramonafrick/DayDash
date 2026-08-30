using System;
using System.Collections.Generic;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.Camera.Application.Contracts;

public interface ILearningGoalParser
{
    List<LearningGoal> ParseToLearningGoals(string ocrText, Guid examId);
}