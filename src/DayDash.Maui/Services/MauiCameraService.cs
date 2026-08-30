using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Camera.Application.Contracts;

namespace DayDash.Maui.Services;

/// <summary>
/// Android implementation of <see cref="ICameraService"/>.
/// Captures a photo with the platform camera. On-device OCR (Google ML Kit Text
/// Recognition v2, offline) is not wired up yet - see Requirements.md §5.2.
/// </summary>
public class MauiCameraService : ICameraService
{
	public async Task<string> CaptureAndRecognizeTextAsync(CancellationToken ct = default)
	{
		if (!MediaPicker.Default.IsCaptureSupported)
		{
			return string.Empty;
		}

		var photo = await MediaPicker.Default.CapturePhotoAsync();
		if (photo is null)
		{
			return string.Empty;
		}

		// TODO: run the captured image through ML Kit Text Recognition v2 (offline)
		// and return the recognised text line by line.
		return string.Empty;
	}
}
