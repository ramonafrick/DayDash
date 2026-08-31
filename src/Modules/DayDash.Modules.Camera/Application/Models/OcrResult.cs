namespace DayDash.Modules.Camera.Application.Models;

/// <summary>
/// Outcome of a single "capture a photo and run on-device OCR" attempt. The camera
/// service maps every failure - permission, cancellation, missing hardware, ML Kit
/// error - onto one of these values and never throws.
/// </summary>
public enum OcrCaptureStatus
{
    /// <summary>A photo was taken and at least one non-empty text line was recognised.</summary>
    Success,

    /// <summary>A photo was taken but ML Kit found no readable text.</summary>
    NoTextFound,

    /// <summary>The user backed out of the camera without taking a photo.</summary>
    Cancelled,

    /// <summary>The camera permission was denied.</summary>
    PermissionDenied,

    /// <summary>This platform has no camera / on-device OCR (e.g. the browser preview).</summary>
    NotSupported,

    /// <summary>The capture or recognition failed unexpectedly.</summary>
    Failed,
}

/// <summary>Result of <see cref="Contracts.ICameraService.CaptureAndRecognizeTextAsync"/>.</summary>
/// <param name="Status">What happened.</param>
/// <param name="Text">The recognised text (newline-separated lines) when <see cref="Status"/> is
/// <see cref="OcrCaptureStatus.Success"/>; otherwise empty.</param>
public sealed record OcrResult(OcrCaptureStatus Status, string Text)
{
    public static OcrResult Success(string text) => new(OcrCaptureStatus.Success, text);

    public static OcrResult Failure(OcrCaptureStatus status) => new(status, string.Empty);
}
