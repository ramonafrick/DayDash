using System.Globalization;
using DayDash.Modules.Reminder.Application.Contracts;
using DayDash.Modules.Settings.Application.Services;
using DayDash.Modules.Settings.UI.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DayDash.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Apply the persisted UI culture before the first render (de-CH by default).
		var stored = Preferences.Get(LanguageSelectorComponent.CulturePreferenceKey, SupportedCultures.Default);
		var culture = SupportedCultures.Resolve(stored);
		CultureInfo.DefaultThreadCurrentCulture = culture;
		CultureInfo.DefaultThreadCurrentUICulture = culture;
		CultureInfo.CurrentCulture = culture;
		CultureInfo.CurrentUICulture = culture;
	}

	protected override Window CreateWindow(IActivationState? activationState)
	{
		return new Window(new MainPage()) { Title = "DayDash" };
	}

	protected override void OnStart()
	{
		base.OnStart();

		// Alarms don't survive a reboot and the plan can change while the app is closed -
		// re-derive the whole schedule now that the DB is ready and the culture is applied.
		_ = RescheduleRemindersAsync();
	}

	private static async Task RescheduleRemindersAsync()
	{
		var services = IPlatformApplication.Current?.Services;
		if (services is null)
		{
			return;
		}

		try
		{
			using var scope = services.CreateScope();
			await scope.ServiceProvider.GetRequiredService<IReminderService>().RescheduleAllAsync();
		}
		catch (Exception ex)
		{
			services.GetService<ILoggerFactory>()?.CreateLogger("Startup")
				.LogError(ex, "Reminder rescheduling failed");
		}
	}
}
