# DayDash – GitHub Copilot Instructions

## Project Overview

DayDash is a local-first children's calendar and study planner app.  
Stack: **.NET 10 / MAUI + Blazor Hybrid** | **C# 13** | **SQLite + EF Core**  
Architecture: **Modular Clean Architecture** (same pattern as MiniMate)

The app UI is available in **German (Switzerland) and English**.  
Localization uses `.resx` files per module (same concept as MiniMate).  
All code, comments, variables, method names, and instruction files are **exclusively in English**.

---

## Solution Structure

```
src/
├── DayDash.slnx
├── DayDash.Maui/               # MAUI Host with Blazor Hybrid
├── DayDash.Web/                # Blazor WASM (dev/preview)
└── Modules/
    └── DayDash.Modules.{Name}/
        ├── Application/
        │   ├── Contracts/      # Interfaces only (e.g. IExamService)
        │   ├── Models/         # DTOs, Request/Response models
        │   └── Services/       # Use-case implementations
        ├── Domain/             # Entities, Enums, Value Objects
        ├── Infrastructure/     # DB, external libs, platform-specific code
        ├── Resources/          # .resx localization files
        │   ├── {Name}Resources.resx       # Default (German CH)
        │   ├── {Name}Resources.en.resx    # English
        │   └── {Name}Resources.Designer.cs
        ├── UI/Components/      # Razor components
        └── {Name}ModuleExtensions.cs
```

---

## Modular Architecture Rules

- Every module is a standalone `.csproj` (Class Library)
- Modules communicate **only through interfaces** defined in `Application/Contracts/`
- No direct references between module concrete classes
- Each module self-registers via `{Name}ModuleExtensions.cs`:

```csharp
public static class CalendarModuleExtensions
{
    public static IServiceCollection AddDayDashCalendar(this IServiceCollection services)
    {
        services.AddScoped<ICalendarService, CalendarService>();
        services.AddScoped<ICalendarRepository, CalendarRepository>();
        return services;
    }
}
```

- `MauiProgram.cs` calls all module extensions:

```csharp
builder.Services
    .AddDayDashStorage()
    .AddDayDashCalendar()
    .AddDayDashStudyPlanner()
    .AddDayDashReminder()
    .AddDayDashCamera()
    .AddDayDashWidget();
```

---

## C# Code Standards

### General Rules
- **C# 13** – prefer modern features
- **Primary Constructors** where appropriate
- **Collection Expressions** `[]` instead of `new List<>()` or `Array.Empty<>()`
- **SOLID Principles** throughout
- **Dependency Injection** everywhere – no static service classes
- All identifiers, comments, and documentation: **English only**

### Code Examples

```csharp
// ✅ Primary Constructor
public class ExamService(IExamRepository repository, ILogger<ExamService> logger) : IExamService
{
    public async Task<Exam?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await repository.GetByIdAsync(id, ct);
}

// ✅ Collection Expression
private readonly string[] _defaultSubjects = ["Mathematics", "German", "English"];

// ✅ Pattern Matching
if (calendarEvent is { EventType: EventType.Exam, LinkedExamId: not null } examEvent)
{
    await LoadStudyPlanAsync(examEvent.LinkedExamId.Value, ct);
}

// ❌ Avoid
var list = new List<string>();
var arr = Array.Empty<string>();
```

### Async / Await
- Always include `CancellationToken ct = default` as last parameter
- Never use `async void` – except Blazor event handlers (`@onclick`)
- Use `ConfigureAwait(false)` in module library code

### Naming Conventions
```
Interfaces:     ICalendarService, IExamRepository
Services:       CalendarService, ExamService
Repositories:   ExamRepository
Entities:       Exam, CalendarEvent, LearningGoal
DTOs / Models:  ExamDto, CreateExamRequest, StudyPlanResponse
Extensions:     CalendarModuleExtensions
Razor:          ExamCardComponent.razor, CalendarMonthView.razor
Resources:      CalendarResources.resx, CalendarResources.en.resx
```

---

## Localization (resx)

Every module has its own resource files. The app supports:
- `de-CH` – German Switzerland (default)
- `en` – English

### Resource File Structure per Module
```
Resources/
├── {Name}Resources.resx           # Default strings (German CH)
├── {Name}Resources.en.resx        # English strings
└── {Name}Resources.Designer.cs    # Auto-generated accessor class
```

### Usage in Razor Components
```razor
@inject IStringLocalizer<CalendarResources> Loc

<h2>@Loc["ExamTitle"]</h2>
<p>@Loc["DaysRemaining", daysLeft]</p>
```

### Usage in Services
```csharp
public class ExamService(IStringLocalizer<StudyPlannerResources> localizer) : IExamService
{
    private string GetRecommendationText(int goals, int minutes)
        => localizer["StudyRecommendation", goals, minutes];
}
```

### Language Switch (Settings)
- User can switch language in app settings
- Selection saved locally via `IPreferences` (MAUI Essentials)
- Culture applied at app startup in `MauiProgram.cs`

---

## Blazor Component Standards

### File Structure
```
UI/Components/
├── ExamCard.razor
├── ExamCard.razor.cs      # Code-behind (partial class)
└── ExamCard.razor.css     # Scoped CSS
```

### Component Pattern
```razor
@* ExamCard.razor *@
@inject IStringLocalizer<StudyPlannerResources> Loc

<div class="exam-card">
    <h3>@Exam.Title</h3>
    <span>@Loc["DaysLeft", DaysRemaining]</span>
    <button @onclick="HandleSelectAsync">@Loc["Details"]</button>
</div>
```

```csharp
// ExamCard.razor.cs
public partial class ExamCard
{
    [Parameter] public required Exam Exam { get; set; }
    [Parameter] public EventCallback<Guid> OnExamSelected { get; set; }

    [Inject] private IExamService ExamService { get; set; } = default!;

    private int DaysRemaining => (Exam.ExamDate.ToDateTime(TimeOnly.MinValue) - DateTime.Today).Days;

    private async Task HandleSelectAsync()
        => await OnExamSelected.InvokeAsync(Exam.Id);
}
```

---

## EF Core / SQLite

- Repository Pattern: Interface in `Application/Contracts/`, implementation in `Infrastructure/`
- Shared `DbContext` lives in the `Storage` module
- Always generate migrations: `dotnet ef migrations add {MigrationName}`
- No raw SQL unless performance-critical
- Repositories should prefer returning IReadOnlyList<T> or IEnumerable<T> for read operations.

---

## MAUI Platform-Specific

- Platform code goes in `Platforms/Android/` inside the MAUI project
- Platform features (camera, notifications, widget) are abstracted via interfaces in the module's `Application/Contracts/`
- The MAUI project provides the concrete implementation in `DayDash.Maui/Services/`

### Example: Camera Abstraction
```csharp
// In DayDash.Modules.Camera/Application/Contracts/
public interface ICameraService
{
    Task<string> CaptureAndRecognizeTextAsync(CancellationToken ct = default);
}

// In DayDash.Maui/Services/
public class MauiCameraService : ICameraService
{
    // MAUI + ML Kit implementation
}
```

---

## Modules Overview

| Module | Purpose |
|---|---|
| `Calendar` | Monthly/weekly calendar view, event management |
| `Camera` | OCR text recognition from photos (ML Kit offline) |
| `StudyPlanner` | Exam creation, learning goals, daily study split |
| `Reminder` | Push notifications, daily study reminder at 15:30 |
| `Widget` | Android home screen widget (day/week/month) |
| `Storage` | SQLite persistence, EF Core, .ics export |

---

## Copilot Reminders

- Always ask: "Does this belong in Application, Domain, or Infrastructure?"
- Never instantiate services directly – always use DI
- No `static` helper classes – use extension methods on relevant types
- Every new entity needs an interface + repository
- Every new module needs `{Name}ModuleExtensions.cs`
- Every user-facing string must go through a `.resx` resource file

## Interaction Rules
- Before writing code, briefly state which layer (Domain, Application, Infrastructure) you are targeting.
- When creating a new file, always provide the full path based on the "Solution Structure".
- If a change affects multiple modules, remind me to update the respective ModuleExtensions.