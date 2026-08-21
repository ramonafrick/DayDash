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
		builder.Services.AddDayDashCamera();
		builder.Services.AddScoped<ICameraService, MauiCameraService>();
		builder.Services.AddScoped<IReminderService, MauiReminderService>();

		builder.Services
			.AddDayDashStorage()
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
