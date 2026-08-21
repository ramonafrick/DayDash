using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.StudyPlanner.Domain;

namespace DayDash.Modules.Camera.UI.Components;

public partial class CameraCaptureComponent
{
    private bool IsLoading { get; set; } = false;
    private List<LearningGoal> ParsedGoals { get; set; } = new();

    private async Task CapturePhotoAsync()
    {
        IsLoading = true;
        try
        {
            var ocrText = await CameraService.CaptureAndRecognizeTextAsync();
            ParsedGoals = LearningGoalParser.ParseToLearningGoals(ocrText, Guid.NewGuid());
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void HandleGoalsSaved(List<LearningGoal> goals)
    {
        ParsedGoals = goals;
    }
}