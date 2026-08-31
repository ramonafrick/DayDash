using System;
using System.Threading;
using System.Threading.Tasks;
using Android.Graphics;
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

            await using var stream = await photo.OpenReadAsync();
            using var bitmap = await BitmapFactory.DecodeStreamAsync(stream);
            if (bitmap is null)
            {
                return OcrResult.Failure(OcrCaptureStatus.Failed);
            }

            var image = InputImage.FromBitmap(bitmap, 0);
            using var recognizer = TextRecognition.GetClient(TextRecognizerOptions.DefaultOptions);

            var listener = new MlKitTaskCompletionListener();
            using var task = recognizer.Process(image);
            task.AddOnCompleteListener(listener);

            var result = await listener.Task.WaitAsync(ct);
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
