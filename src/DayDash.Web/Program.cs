using System.Globalization;
using DayDash.Modules.Calendar;
using DayDash.Modules.Camera;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Reminder;
using DayDash.Modules.Settings;
using DayDash.Modules.Settings.Application.Contracts;
using DayDash.Modules.Settings.Application.Services;
using DayDash.Modules.Storage;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.StudyPlanner;
using DayDash.Modules.Widget;
using DayDash.Web;
using DayDash.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddLocalization();
builder.Services.AddSingleton(TimeProvider.System);

// Browser adapters for the platform contracts (FR-P4: previews degrade gracefully).
builder.Services.AddScoped<IAppPreferences, LocalStorageAppPreferences>();
builder.Services.AddScoped<IFileShareService, BrowserDownloadService>();
builder.Services.AddScoped<ICameraService, WebCameraService>();

// Feature modules - Settings and Storage first (both leaves).
builder.Services
    .AddDayDashSettings()
    .AddDayDashStorage(options => options.UseInMemoryDatabase("DayDash-Preview"))
    .AddDayDashCalendar()
    .AddDayDashStudyPlanner()
    .AddDayDashCamera()
    .AddDayDashReminder()
    .AddDayDashWidget();

var host = builder.Build();

var storedCulture = await host.Services.GetRequiredService<IAppPreferences>()
    .GetAsync(DayDash.Modules.Settings.UI.Components.LanguageSelectorComponent.CulturePreferenceKey);
var culture = SupportedCultures.Resolve(storedCulture);
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

using (var scope = host.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>().InitializeAsync();
}

await host.RunAsync();
