namespace DayDash.Modules.Settings.Application.Contracts;

/// <summary>
/// Host-provided file delivery. On Android this raises the share sheet; in the browser
/// preview it triggers a download. Used e.g. by the calendar module's <c>.ics</c> export.
/// </summary>
public interface IFileShareService
{
    Task ShareFileAsync(string filePath, string title, CancellationToken ct = default);
}
