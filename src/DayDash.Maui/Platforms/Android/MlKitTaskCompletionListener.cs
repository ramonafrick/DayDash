using System.Threading.Tasks;
using Android.Gms.Tasks;

namespace DayDash.Maui.Platforms.Android;

/// <summary>
/// Bridges a Google Play Services <see cref="global::Android.Gms.Tasks.Task"/> to a .NET
/// <see cref="System.Threading.Tasks.Task{TResult}"/> via a <see cref="TaskCompletionSource{TResult}"/>.
/// ML Kit hands back a Java task; this lets the camera service <c>await</c> it.
/// </summary>
internal sealed class MlKitTaskCompletionListener : Java.Lang.Object, IOnCompleteListener
{
    private readonly TaskCompletionSource<Java.Lang.Object?> _tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public System.Threading.Tasks.Task<Java.Lang.Object?> Task => _tcs.Task;

    public void OnComplete(global::Android.Gms.Tasks.Task task)
    {
        if (task.IsSuccessful)
        {
            _tcs.TrySetResult(task.Result);
        }
        else
        {
            _tcs.TrySetException(task.Exception ?? new Java.Lang.Exception("ML Kit text recognition failed."));
        }
    }
}
