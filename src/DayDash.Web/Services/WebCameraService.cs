using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Camera.Application.Models;

namespace DayDash.Web.Services;

/// <summary>
/// The browser preview has no camera / on-device OCR. Returns <see cref="OcrCaptureStatus.NotSupported"/>
/// so the component renders its "not available in the browser preview" hint instead of failing (FR-P4).
/// </summary>
public sealed class WebCameraService : ICameraService
{
    public Task<OcrResult> CaptureAndRecognizeTextAsync(CancellationToken ct = default)
        => Task.FromResult(OcrResult.Failure(OcrCaptureStatus.NotSupported));
}
