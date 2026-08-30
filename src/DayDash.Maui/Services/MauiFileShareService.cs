using DayDash.Modules.Settings.Application.Contracts;

namespace DayDash.Maui.Services;

/// <summary>Raises the Android share sheet for a generated file (e.g. the .ics export).</summary>
public sealed class MauiFileShareService : IFileShareService
{
	public Task ShareFileAsync(string filePath, string title, CancellationToken ct = default)
		=> Share.Default.RequestAsync(new ShareFileRequest
		{
			Title = title,
			File = new ShareFile(filePath),
		});
}
