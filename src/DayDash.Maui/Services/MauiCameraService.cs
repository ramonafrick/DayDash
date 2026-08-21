using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Camera.Application.Contracts;
using Microsoft.Maui.Media;
using Xamarin.Google.MLKit.Vision.TextRecognition;

namespace DayDash.Maui.Services;

public class MauiCameraService : ICameraService
{
    public async Task<string> CaptureAndRecognizeTextAsync(CancellationToken ct = default)
    {
        // Capture photo using MediaPicker
        var photo = await MediaPicker.CapturePhotoAsync();
        if (photo == null)
        {
            return string.Empty;
        }

        // Read the photo stream
        using var stream = await photo.OpenReadAsync();
        var tempFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);
        using (var fileStream = File.Create(tempFilePath))
        {
            await stream.CopyToAsync(fileStream);
        }

        // Process the image with ML Kit Text Recognizer
        var textRecognizer = TextRecognition.GetClient();
        var visionImage = VisionImage.FromFilePath(tempFilePath);
        var result = await textRecognizer.Process(visionImage);

        // Extract recognized text
        return result.Text;
    }
}