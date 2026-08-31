using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Camera.Application.Models;

namespace DayDash.Tests.Infrastructure;

/// <summary>Test double for <see cref="ICameraService"/> - returns a scripted <see cref="OcrResult"/>.</summary>
public sealed class FakeCameraService : ICameraService
{
    public OcrResult Next { get; set; } = OcrResult.Failure(OcrCaptureStatus.Cancelled);

    public int Calls { get; private set; }

    public Task<OcrResult> CaptureAndRecognizeTextAsync(CancellationToken ct = default)
    {
        Calls++;
        return Task.FromResult(Next);
    }
}
