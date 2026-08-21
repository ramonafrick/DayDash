using System.Threading;
using System.Threading.Tasks;

namespace DayDash.Modules.Camera.Application.Contracts;

public interface ICameraService
{
    Task<string> CaptureAndRecognizeTextAsync(CancellationToken ct = default);
}