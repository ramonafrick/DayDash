namespace DayDash.Maui;

/// <summary>
/// Carries a fatal startup error (currently only DB initialization) so the UI can show a
/// localized banner instead of a blank screen. Never used to delete user data.
/// </summary>
public static class StartupDiagnostics
{
	public static Exception? DatabaseError { get; set; }
}
