# CLAUDE.md

This file provides guidance to Claude Code when working with code in this repository.

## Project Overview

DayDash is a modular children's calendar and study planner app built with .NET MAUI + Blazor Hybrid (.NET 10).  
It is designed for children aged 8–14 to manage exams, learning goals, and daily schedules.  
The project follows a fully modular Clean Architecture – each module is an independent `.csproj` that can be added or removed without touching other modules.

The app supports **German (Switzerland)** and **English**. All code is written in English.

---

## Build & Run Commands

### Build entire solution
```bash
dotnet build src/DayDash.slnx
```

### Run MAUI app (Android emulator or device)
```bash
cd src/DayDash.Maui
dotnet build -f net10.0-android -t:Run
```

### Run Web project (browser dev)
```bash
cd src/DayDash.Web
dotnet run
```

### Restore dependencies
```bash
dotnet restore src/DayDash.slnx
```

### Clean build artifacts
```bash
dotnet clean src/DayDash.slnx
```

### EF Core Migrations (run from src/)
```bash
dotnet ef migrations add {MigrationName} --project Modules/DayDash.Modules.Storage
dotnet ef database update --project Modules/DayDash.Modules.Storage
```

---

## Project Structure

```
src/
├── DayDash.slnx
├── DayDash.Maui/               # MAUI Host (Blazor Hybrid, Android)
│   ├── Components/             # Shared Blazor layout components
│   ├── Pages/                  # XAML pages (shell navigation)
│   ├── Platforms/Android/      # Android-specific code (widget, notifications)
│   ├── Services/               # MAUI implementations of module interfaces
│   ├── wwwroot/                # Static web assets
│   └── MauiProgram.cs          # App entry point, DI registration
├── DayDash.Web/                # Blazor WASM (browser preview)
└── Modules/
    ├── DayDash.Modules.Calendar/
    ├── DayDash.Modules.Camera/
    ├── DayDash.Modules.StudyPlanner/
    ├── DayDash.Modules.Reminder/
    ├── DayDash.Modules.Widget/
    └── DayDash.Modules.Storage/
```

---

## Module Architecture

Each module follows this structure:

```
DayDash.Modules.{Name}/
├── Application/
│   ├── Contracts/          # Service interfaces (e.g. IExamService, IExamRepository)
│   ├── Models/             # DTOs and request/response models
│   └── Services/           # Use-case implementations
├── Domain/                 # Entities, Enums, Value Objects (no external dependencies)
├── Infrastructure/         # EF Core, external libs, platform adapters
├── Resources/
│   ├── {Name}Resources.resx        # Default (German CH)
│   ├── {Name}Resources.en.resx     # English
│   └── {Name}Resources.Designer.cs
├── UI/
│   └── Components/         # Razor components (.razor + .razor.cs + .razor.css)
└── {Name}ModuleExtensions.cs       # DI self-registration
```

### Key Architectural Patterns

1. **Module self-registration** – each module exposes one extension method:
   ```csharp
   services.AddDayDashCalendar();
   ```

2. **Interface-only coupling** – modules never reference each other's concrete classes

3. **Code-behind pattern** – Razor components use `.razor.cs` partial classes

4. **CSS isolation** – each component has its own `.razor.css`

5. **Localization** – every user-facing string is in a `.resx` file, never hardcoded

---

## Current Modules

| Module | Key Responsibility |
|---|---|
| `Calendar` | CalendarEvent CRUD, month/week view, .ics export |
| `Camera` | Photo capture + ML Kit OCR (offline), learning goal extraction |
| `StudyPlanner` | Exam management, study time calculation, daily split |
| `Reminder` | Android push notifications, daily 15:30 reminder |
| `Widget` | Android home screen widget (day/week/month views) |
| `Storage` | SQLite via EF Core, shared DbContext, repository base classes |

---

## Key Implementation Details

### Storage
- SQLite database via EF Core
- `DayDashDbContext` lives in `DayDash.Modules.Storage/Infrastructure/`
- Repository interfaces in `Application/Contracts/`, implementations in `Infrastructure/`
- Database file stored in `FileSystem.AppDataDirectory` (MAUI Essentials)

### Localization
- Default culture: `de-CH` (German Switzerland)
- Secondary culture: `en` (English)
- Culture set at startup based on `IPreferences` value
- `IStringLocalizer<T>` used throughout services and components

### Camera / OCR
- Google ML Kit Text Recognition v2 – fully offline, no API costs
- Platform-specific implementation in `DayDash.Maui/Services/MauiCameraService.cs`
- Interface `ICameraService` in `DayDash.Modules.Camera/Application/Contracts/`

### Notifications
- Android Notification Channels via MAUI
- Default daily reminder: 15:30, configurable in settings
- `IReminderService` interface in `DayDash.Modules.Reminder/Application/Contracts/`

### Widget
- Android `AppWidgetProvider` in `DayDash.Maui/Platforms/Android/`
- Three sizes: day overview, week overview, month overview

---

## Adding a New Module

1. Create the module directory structure (see above)
2. Define service interface in `Application/Contracts/`
3. Create domain entities in `Domain/`
4. Implement the service in `Application/Services/`
5. Add EF Core entity configuration in `Infrastructure/` and register with `DayDashDbContext`
6. Create Razor components in `UI/Components/`
7. Add resource files in `Resources/` for both `de-CH` and `en`
8. Create `{Name}ModuleExtensions.cs` with `AddDayDash{Name}(this IServiceCollection services)`
9. Register the module in `DayDash.Maui/MauiProgram.cs`
10. Add navigation entry in `AppShell.xaml` if needed

---

## Technology Stack

| Area | Technology |
|---|---|
| Framework | .NET 10 / MAUI + Blazor Hybrid |
| Language | C# 13 |
| UI | Blazor Razor Components |
| Database | SQLite + EF Core |
| OCR | Google ML Kit Text Recognition v2 (offline) |
| Notifications | MAUI / Android Notification Channels |
| Widget | Android AppWidgetProvider |
| Localization | .resx + IStringLocalizer |
| DI | Microsoft.Extensions.DependencyInjection |
| Export | iCalendar (.ics) |

## Configuration Notes

- **Nullable Reference Types**: Enabled
- **Implicit Usings**: Enabled
- **Primary Constructors**: Preferred over constructor injection pattern
- **Collection Expressions**: Preferred (`[]` over `new List<>()`)
- **Target Framework**: net10.0 / net10.0-android
- **C# Version**: C# 13 (with .NET 10)
