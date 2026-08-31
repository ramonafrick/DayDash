using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Camera.Application.Models;

namespace DayDash.Modules.Camera.Application.Contracts;

/// <summary>
/// Captures a photo with the device camera and runs offline OCR on it. Implementations
/// map every failure onto <see cref="OcrCaptureStatus"/> and never throw.
/// </summary>
public interface ICameraService
{
    Task<OcrResult> CaptureAndRecognizeTextAsync(CancellationToken ct = default);
}
