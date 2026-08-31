using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DayDash.Modules.Camera.Application.Models;
using DayDash.Modules.StudyPlanner.Domain;
using Microsoft.AspNetCore.Components;

namespace DayDash.Modules.Camera.UI.Components;

public partial class CameraCaptureComponent
{
    /// <summary>Optional exam to attach recognised goals to. When null the user picks one from the list.</summary>
    [Parameter] public Guid? ExamId { get; set; }

    private readonly List<Exam> _exams = [];
    private Guid _selectedExamId;
    private bool _busy;
    private bool _saving;
    private bool _loadFailed;
    private OcrCaptureStatus? _status;
    private List<LearningGoal> _goals = [];
    private SaveResult _saveState;

    private enum SaveResult
    {
        None,
        Saved,
        Failed,
    }

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var exams = await StudyPlanner.GetExamsAsync();
            _exams.Clear();
            _exams.AddRange(exams);

            _selectedExamId = ExamId is { } id && _exams.Any(e => e.Id == id)
                ? id
                : _exams.FirstOrDefault()?.Id ?? Guid.Empty;
        }
        catch
        {
            _loadFailed = true;
        }
    }

    /// <summary>Selecting a different exam discards a scan that belonged to the previous one.</summary>
    private void ResetScan()
    {
        _status = null;
        _goals = [];
        _saveState = SaveResult.None;
    }

    private async Task CaptureAsync()
    {
        if (_selectedExamId == Guid.Empty || _busy)
        {
            return;
        }

        _busy = true;
        _status = null;
        _saveState = SaveResult.None;
        _goals = [];

        try
        {
            var result = await CameraService.CaptureAndRecognizeTextAsync();
            _status = result.Status;

            if (result.Status == OcrCaptureStatus.Success)
            {
                _goals = LearningGoalParser.ParseToLearningGoals(result.Text, _selectedExamId);
                if (_goals.Count == 0)
                {
                    _status = OcrCaptureStatus.NoTextFound;
                }
            }
        }
        catch
        {
            _status = OcrCaptureStatus.Failed;
        }
        finally
        {
            _busy = false;
        }
    }

    private async Task SaveGoalsAsync(List<LearningGoal> goals)
    {
        if (_saving)
        {
            return;
        }

        _saving = true;
        try
        {
            await StudyPlanner.SaveLearningGoalsAsync(_selectedExamId, goals);
            _saveState = SaveResult.Saved;
        }
        catch
        {
            _saveState = SaveResult.Failed;
        }
        finally
        {
            _saving = false;
        }
    }
}
