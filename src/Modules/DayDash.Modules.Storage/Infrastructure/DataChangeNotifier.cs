using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DayDash.Modules.Storage.Infrastructure;

/// <summary>
/// Fans a <see cref="DataChange"/> out to every registered <see cref="IDataChangeHandler"/>.
/// Each handler is isolated: an exception is logged and swallowed so it can never break the
/// write that triggered the notification.
/// Handlers are resolved lazily per notification (not injected in the constructor): handlers
/// may depend on services that themselves publish through <see cref="IDataChangeNotifier"/>
/// (e.g. Reminder → StudyPlanner → SubjectConfig), which would otherwise be a circular
/// constructor dependency.
/// </summary>
public sealed class DataChangeNotifier(
    IServiceProvider services,
    ILogger<DataChangeNotifier> logger) : IDataChangeNotifier
{
    public async Task NotifyAsync(DataChange change, CancellationToken ct = default)
    {
        foreach (var handler in services.GetServices<IDataChangeHandler>())
        {
            try
            {
                await handler.HandleAsync(change, ct);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Data change handler {Handler} failed for {Kind} {EntityId}",
                    handler.GetType().Name, change.Kind, change.EntityId);
            }
        }
    }
}
