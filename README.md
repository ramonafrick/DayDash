# DayDash 📅

A modular children's calendar and study planner, built with .NET MAUI + Blazor Hybrid – designed to help kids aged 8–14 plan exams, track learning goals, and organize their school day.

## What is DayDash?
DayDash is a local, privacy-friendly Android app that combines several practical modules in a unified, kid-friendly interface. Each module is developed independently and can be added or removed without touching the others. Everything works offline – no login, no cloud, no cost. The goal is to help children learn to plan ahead and stay on top of school on their own.

## Current Modules:
- 📅 **Calendar** – Month and week view, configurable event types, colour coding, `.ics` export
- 📷 **Camera (OCR)** – Snap a photo of a learning-goals sheet; ML Kit turns each line into a checkable goal
- 📚 **StudyPlanner** – Exam management, study-time calculation, and daily split across the remaining days
- 🔔 **Reminder** – Daily study reminder (default 15:30) and event reminders via Android notifications
- 📱 **Widget** – Android home-screen widget with day, week, and month views
- 💾 **Storage** – Shared SQLite persistence via EF Core, repository pattern, provider-agnostic
- ⚙️ **Settings** – App-wide culture state, live language switching, and the shared shell (`MainLayout` / `NavMenu` / `ThemeToggle`)

EF Core migrations live in the dedicated `DayDash.Migrations` project (not in `Storage`), so `Storage` stays free of a concrete database provider.

## 🛠️ Technology Stack

**Frontend & Framework**
- .NET 10
- .NET MAUI + Blazor Hybrid (Android)
- Blazor WebAssembly (`DayDash.Web`, browser preview)
- C# 13

**Architecture & Patterns**
- Clean Architecture (Domain, Application, Infrastructure, UI)
- Fully modular – each module is an independent `.csproj`, coupled through interfaces only
- Code-behind pattern (`.razor.cs`)
- CSS Isolation

**Features**
- Localization (German CH / English) via `.resx` + `IStringLocalizer`, live switching without restart
- Offline-first, local-only storage (SQLite via EF Core)
- iCalendar (`.ics`) export for migration to Google / Apple Calendar

**Platform Integration**
- Google ML Kit Text Recognition v2 (fully offline OCR, no API cost)
- Android Notification Channels
- Android home-screen widget (`AppWidgetProvider`)

**No external APIs** – all data stays on the device.

## 🏗️ Architecture

**Clean Architecture Layers:**
- **Domain** – Entities, enums, and value objects; no external dependencies
- **Application** – Service contracts, DTOs, and use-case implementations
- **Infrastructure** – EF Core configuration, external libraries, platform adapters
- **UI** – Blazor components with the code-behind pattern

**Module Structure:**
Each module is a self-contained project with its own layers, `.resx` resources, and a single DI entry point (`AddDayDash{Name}()`). Modules never reference each other's concrete types. `Storage` and `Settings` are the two leaf modules; every UI module references `Settings`, and the entity-owning modules contribute their EF Core model configuration to the shared `DayDashDbContext`.

```
src/
├── DayDash.slnx
├── DayDash.Maui/                # MAUI host (Blazor Hybrid, Android) – single BlazorWebView
├── DayDash.Web/                 # Blazor WASM (browser preview, EF Core InMemory)
├── DayDash.Migrations/          # EF Core migrations + design-time / widget DbContext factory
└── Modules/
    ├── DayDash.Modules.Calendar/
    ├── DayDash.Modules.Camera/
    ├── DayDash.Modules.StudyPlanner/
    ├── DayDash.Modules.Reminder/
    ├── DayDash.Modules.Widget/
    ├── DayDash.Modules.Settings/
    └── DayDash.Modules.Storage/
tests/
└── DayDash.Tests/               # xUnit + bUnit, SQLite :memory: fixture
```

## ✨ Features:
- 📱 Android smartphone & tablet
- 🔒 Offline-first – no internet required, no external API calls
- 👶 Kid-friendly interface for ages 8–14
- 🗓️ Month / week calendar with tap-to-detail and configurable, colour-coded event types
- 📷 Photo-to-text learning goals via offline OCR, editable line by line
- 📚 Automatic study-time recommendation (goals × minutes/goal) and daily split until the exam
- ⚙️ Configurable subjects and minutes-per-goal
- 🔔 Daily study reminder that only fires when study time is planned, plus per-event reminders
- 🧩 Home-screen widget in three sizes (day / week / month)
- 🌍 German (Switzerland) and English, switchable in settings without restart
- 📤 `.ics` export for later migration to another calendar
- 🧱 Extensible – add a new module without changing the core

## Build & Run

```bash
# Build the whole solution
dotnet build src/DayDash.slnx

# Run the test suite (test the test project, not the slnx)
dotnet test tests/DayDash.Tests/DayDash.Tests.csproj

# Run the MAUI app on an Android emulator or device
dotnet build src/DayDash.Maui/DayDash.Maui.csproj -f net10.0-android -t:Run

# Run the browser preview
dotnet run --project src/DayDash.Web
```

### EF Core migrations

Migrations target the `DayDash.Migrations` project (it is both `--project` and `--startup-project`):

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project src/DayDash.Migrations --startup-project src/DayDash.Migrations --output-dir Migrations
dotnet ef migrations list       --project src/DayDash.Migrations --startup-project src/DayDash.Migrations
```

On the device the database is created and upgraded automatically on first run
(`IDatabaseInitializer` runs `Migrate()` + the module seeders).

## License
MIT – see [LICENSE](LICENSE).
