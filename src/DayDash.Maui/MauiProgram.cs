using DayDash.Maui.Services;
using DayDash.Modules.Calendar;
using DayDash.Modules.Camera;
using DayDash.Modules.Camera.Application.Contracts;
using DayDash.Modules.Reminder;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Storage;
using DayDash.Modules.StudyPlanner;
using DayDash.Modules.Widget;
using Microsoft.Extensions.Logging;

namespace DayDash.Maui;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
			});

		builder.Services.AddMauiBlazorWebView();
		builder.Services.AddLocalization();

		// Platform-specific implementations of module contracts.
		builder.Services.AddSingleton<ICameraService, MauiCameraService>();
		builder.Services.AddSingleton<IReminderService, MauiReminderService>();

		// Feature modules (each self-registers via its extension method).
		builder.Services
			.AddDayDashStorage(Path.Combine(FileSystem.AppDataDirectory, "DayDash.db"))
			.AddDayDashCalendar()
			.AddDayDashStudyPlanner()
			.AddDayDashReminder()
			.AddDayDashCamera()
			.AddDayDashWidget();

#if DEBUG
		builder.Services.AddBlazorWebViewDeveloperTools();
		builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
