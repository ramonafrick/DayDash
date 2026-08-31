using DayDash.Maui.Services;
using DayDash.Migrations;
using DayDash.Modules.Calendar;
using DayDash.Modules.Camera;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Reminder;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Settings;
using DayDash.Modules.Settings.Application.Contracts;
using DayDash.Modules.Settings.Application.Services;
using DayDash.Modules.Storage.Application.Contracts;
using DayDash.Modules.Storage.Infrastructure;
using DayDash.Modules.StudyPlanner;
using DayDash.Modules.Widget;
using Microsoft.Extensions.Logging;
using Plugin.LocalNotification;

namespace DayDash.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseLocalNotification()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddLocalization();
		builder.Services.AddSingleton(TimeProvider.System);

		// Platform adapters - registered BEFORE the modules so their fallbacks never win.
		builder.Services.AddSingleton<IAppPreferences, MauiAppPreferences>();
		builder.Services.AddSingleton<IFileShareService, MauiFileShareService>();
		builder.Services.AddScoped<ICameraService, MauiCameraService>();
		builder.Services.AddSingleton<INotificationScheduler, MauiNotificationScheduler>();

		// Feature modules - Settings and Storage first (both leaves).
		builder.Services
			.AddDayDashSettings()
			.AddDayDashSqliteStorage(Path.Combine(FileSystem.AppDataDirectory, DayDashDatabase.FileName))
			.AddDayDashCalendar()
			.AddDayDashStudyPlanner()
			.AddDayDashCamera()
			.AddDayDashReminder()
			.AddDayDashWidget();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		var app = builder.Build();

		using (var scope = app.Services.CreateScope())
		{
			try
			{
				scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>()
					.InitializeAsync().GetAwaiter().GetResult();
			}
			catch (Exception ex)
			{
				app.Services.GetRequiredService<StartupState>().DatabaseError = ex;
				app.Services.GetService<ILoggerFactory>()?.CreateLogger("Startup")
					.LogError(ex, "Database initialization failed");
			}
		}

		// Reminders are (re)armed from App.OnStart, once the UI culture is applied.
		return app;
	}
}
