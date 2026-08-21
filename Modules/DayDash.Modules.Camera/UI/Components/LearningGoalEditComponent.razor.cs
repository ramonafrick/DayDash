using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.Camera.UI.Components;

public partial class LearningGoalEditComponent
{
    [Parameter] public List<LearningGoal> Goals { get; set; } = new();
    [Parameter] public EventCallback<List<LearningGoal>> OnGoalsSaved { get; set; }

    private void AddGoal()
    {
        Goals.Add(new LearningGoal { Title = "", IsChecked = false, SortOrder = Goals.Count });
    }

    private void DeleteGoal(LearningGoal goal)
    {
        Goals.Remove(goal);
        ReorderGoals();
    }

    private void MoveUp(LearningGoal goal)
    {
        var index = Goals.IndexOf(goal);
        if (index > 0)
        {
            (Goals[index - 1], Goals[index]) = (Goals[index], Goals[index - 1]);
            ReorderGoals();
        }
    }

    private void MoveDown(LearningGoal goal)
    {
        var index = Goals.IndexOf(goal);
        if (index < Goals.Count - 1)
        {
            (Goals[index + 1], Goals[index]) = (Goals[index], Goals[index + 1]);
            ReorderGoals();
        }
    }

    private void ReorderGoals()
    {
        for (int i = 0; i < Goals.Count; i++)
        {
            Goals[i].SortOrder = i;
        }
    }

    private async Task SaveGoals()
    {
        await OnGoalsSaved.InvokeAsync(Goals);
    }
}