using System;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Maui.Platforms.Android;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Camera.Application.Models;
using Xamarin.Google.MLKit.Vision.Common;
using Xamarin.Google.MLKit.Vision.Text;
using Xamarin.Google.MLKit.Vision.Text.Latin;

namespace DayDash.Maui.Services;

/// <summary>
/// Android implementation of <see cref="ICameraService"/>: captures a photo with the
/// platform camera and runs Google ML Kit Text Recognition v2 on it, fully offline.
/// Every failure path is mapped to an <see cref="OcrCaptureStatus"/> - this method never throws.
/// </summary>
public sealed class MauiCameraService : ICameraService
{
    public async Task<OcrResult> CaptureAndRecognizeTextAsync(CancellationToken ct = default)
    {
        try
        {
            ct.ThrowIfCancellationRequested();

            if (!MediaPicker.Default.IsCaptureSupported)
            {
                return OcrResult.Failure(OcrCaptureStatus.NotSupported);
            }

            FileResult? photo;
            try
            {
                photo = await MediaPicker.Default.CapturePhotoAsync();
            }
            catch (PermissionException)
            {
                return OcrResult.Failure(OcrCaptureStatus.PermissionDenied);
            }
            catch (FeatureNotSupportedException)
            {
                return OcrResult.Failure(OcrCaptureStatus.NotSupported);
            }

            if (photo is null)
            {
                return OcrResult.Failure(OcrCaptureStatus.Cancelled);
            }

            // InputImage.FromFilePath reads the JPEG's EXIF orientation, so a portrait
            // photo of a worksheet is handed to ML Kit upright rather than sideways.
            var uri = global::Android.Net.Uri.FromFile(new Java.IO.File(photo.FullPath))
                      ?? throw new InvalidOperationException("Captured photo has no file path.");
            var image = InputImage.FromFilePath(global::Android.App.Application.Context, uri);

            using var recognizer = TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions);
            var listener = new MlKitTaskCompletionListener();
            using var task = recognizer.Process(image);
            task.AddOnCompleteListener(listener);

            // Let recognition run to completion - abandoning it mid-flight would dispose
            // the recognizer while ML Kit is still using it natively.
            var result = await listener.Task;
            var text = (result as Text)?.GetText() ?? string.Empty;

            return string.IsNullOrWhiteSpace(text)
                ? OcrResult.Failure(OcrCaptureStatus.NoTextFound)
                : OcrResult.Success(text);
        }
        catch (OperationCanceledException)
        {
            return OcrResult.Failure(OcrCaptureStatus.Cancelled);
        }
        catch (Exception)
        {
            return OcrResult.Failure(OcrCaptureStatus.Failed);
        }
    }
}
