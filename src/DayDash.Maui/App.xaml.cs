using System.Globalization;
using DayDash.Modules.Settings.Application.Services;

namespace DayDash.Maui;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		// Apply the persisted UI culture before the first render (de-CH by default).
		var stored = Preferences.Get("BlazorCulture", SupportedCultures.Default);
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
}
