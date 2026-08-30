using DayDash.Modules.Camera.Application.Contracts;

namespace DayDash.Web.Services;

/// <summary>
/// The browser preview has no camera / on-device OCR. Returns an empty result so the
/// component renders its "nothing recognised" state instead of failing (FR-P4).
/// </summary>
public sealed class WebCameraService : ICameraService
{
    public Task<string> CaptureAndRecognizeTextAsync(CancellationToken ct = default)
        => Task.FromResult(string.Empty);
}
