namespace DayDash.Modules.Settings.Application.Services;

/// <summary>
/// Carries a fatal startup error (currently only database initialization) so the shared
/// <c>MainLayout</c> can show a localized banner instead of leaving the user on a broken
/// screen. Never used to delete or reset user data (see docs/20260830_plan.md, R10).
/// </summary>
public sealed class StartupState
{
    public Exception? DatabaseError { get; set; }

    public bool HasDatabaseError => DatabaseError is not null;
}
