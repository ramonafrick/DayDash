using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.Camera.Application.Services;

/// <summary>
/// Turns raw OCR text into <see cref="LearningGoal"/> rows: one goal per non-blank line,
/// trimmed, internal whitespace runs collapsed to a single space, capped at
/// <see cref="MaxLength"/> characters, with a contiguous <see cref="LearningGoal.SortOrder"/>
/// (blank lines are dropped without leaving gaps).
/// </summary>
public partial class LearningGoalParser : ILearningGoalParser
{
    public const int MaxLength = 200;

    public List<LearningGoal> ParseToLearningGoals(string ocrText, Guid examId)
    {
        if (string.IsNullOrWhiteSpace(ocrText))
        {
            return [];
        }

        return ocrText
            .Split(['\n', '\r'], StringSplitOptions.RemoveEmptyEntries)
            .Select(line => WhitespaceRun().Replace(line, " ").Trim())
            .Where(line => line.Length > 0)
            .Select((line, index) => new LearningGoal
            {
                Id = Guid.NewGuid(),
                ExamId = examId,
                Text = line.Length > MaxLength ? line[..MaxLength] : line,
                SortOrder = index,
                IsChecked = false,
            })
            .ToList();
    }

    [GeneratedRegex(@"\s+")]
    private static partial Regex WhitespaceRun();
}
