using DayDash.Modules.Settings.Application.Contracts;
using Microsoft.JSInterop;

namespace DayDash.Web.Services;

/// <summary>Browser-backed <see cref="IFileShareService"/>: triggers a file download.</summary>
public sealed class BrowserDownloadService(IJSRuntime js) : IFileShareService
{
    public async Task ShareFileAsync(string filePath, string title, CancellationToken ct = default)
    {
        var bytes = await File.ReadAllBytesAsync(filePath, ct);
        var fileName = Path.GetFileName(filePath);
        await js.InvokeVoidAsync("daydashDownload.saveBytes", ct, fileName, Convert.ToBase64String(bytes));
    }
}
