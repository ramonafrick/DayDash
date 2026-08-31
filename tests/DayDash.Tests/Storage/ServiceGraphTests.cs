using DayDash.Modules.Calendar;
using DayDash.Modules.Camera;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Reminder;
using DayDash.Modules.Settings;
using DayDash.Modules.Settings.Application.Contracts;
using DayDash.Modules.Storage;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner;
using DayDash.Modules.Widget;
using DayDash.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DayDash.Tests.Storage;

/// <summary>
/// Regression guard for the full host service graph. The Blazor WASM host validates every
/// scoped service at startup, which is where a circular constructor dependency surfaces
/// (DataChangeNotifier → handlers → Reminder → StudyPlanner → SubjectConfig → notifier).
/// This test composes the same module graph as the hosts and builds it with full validation.
/// </summary>
public class ServiceGraphTests
{
    [Fact]
    public void Full_module_graph_builds_with_validation_and_resolves_the_change_pipeline()
    {
        var services = new ServiceCollection();
        services.AddLocalization();
        services.AddLogging();
        services.AddSingleton(TimeProvider.System);
        services.AddScoped<IAppPreferences, FakeAppPreferences>();
        services.AddScoped<IFileShareService, FakeFileShareService>();
        services.AddScoped<ICameraService, FakeCameraService>();

        services
            .AddDayDashSettings()
            .AddDayDashStorage(options => options.UseInMemoryDatabase("service-graph-validation"))
            .AddDayDashCalendar()
            .AddDayDashStudyPlanner()
            .AddDayDashCamera()
            .AddDayDashReminder()
            .AddDayDashWidget();

        using var provider = services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true,
        });

        using var scope = provider.CreateScope();
        _ = scope.ServiceProvider.GetRequiredService<IDataChangeNotifier>();
        Assert.NotEmpty(scope.ServiceProvider.GetServices<IDataChangeHandler>());
    }
}
