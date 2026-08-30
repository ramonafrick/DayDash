# Requirements: DayDash Full Feature Build-out

**Date**: 2026-08-30
**Status**: awaiting approval → planning (Opus) → implementation (Sonnet, vertical slices)

## Problem

The DayDash solution currently compiles (`dotnet build src/DayDash.slnx` → 0 errors) but is a
skeleton: most features from the product `Requirements.md` are stubs, empty method bodies, or
missing UI. This build-out turns every module into a working feature on Android (MAUI Blazor
Hybrid), with the Blazor WASM host reused for browser-based component development, matching the
architecture and look-and-feel of the sibling project **MiniMate**.

Target user: children aged 8–14 managing exams, learning goals and a daily schedule — fully
offline, no login, no cloud, no cost.

## Users & Context

- **Primary**: a single child (8–14), Android phone or tablet, MAUI app (Blazor Hybrid via
  `BlazorWebView` — native in-process .NET, **not** Blazor Server, **not** WASM). SQLite via
  EF Core on the device file system.
- **Secondary**: the developer, using `DayDash.Web` (standalone Blazor WASM, identical SDK to
  `MiniMate.Web`) as a fast browser preview of the reusable Razor components. In the browser,
  Storage uses **EF Core InMemory** instead of SQLite.
- Single-user, no accounts, no network calls.

## Functional Requirements

### Storage (foundation — slice 1)

- FR-S1: `DayDashDbContext` has typed `DbSet`s for every persisted entity again, with EF Core
  entity configuration (keys, required fields, `SubjectConfig.MinutesPerGoal` default 15).
- FR-S2: The circular dependency between `Storage` and the feature modules is resolved cleanly.
  Entity configuration is contributed by the feature modules (e.g. `IEntityTypeConfiguration<T>`
  applied at model-build time); `Storage` does not reference feature modules. Minimal-invasive
  extra `.csproj`s are allowed if the plan needs them (e.g. a dedicated migrations project).
- FR-S3: Real EF Core **migrations** are checked in. On app start the DB is created/upgraded via
  `Migrate()`.
- FR-S4: A **startup seeder** populates, only when the respective table is empty:
  the six default calendar event types (§5.1: Prüfung, Hausaufgaben [HA], Schulferien,
  Geburtstag, Abmachung, Sonstiges) with their colours; the five default subjects (§5.3:
  Mathematik, Deutsch, NMG, Englisch, Französisch) at 15 min/goal; the default `ReminderConfig`
  (15:30, 1 day before, enabled).
- FR-S5: Repository pattern per aggregate. Generic `BaseRepository<T>` plus feature-specific
  repositories. `.ics` export lives in the Calendar module and produces a valid iCalendar file
  for a set of `CalendarEvent`s.
- FR-S6: In `DayDash.Web` the same repositories run against EF Core InMemory (seeded the same way).

### Calendar (slice 2)

- FR-C1: Month view (default) rendered from real data: a real calendar grid for the current
  month, week rows, day cells, "today" highlight, coloured dots per event type on days with events.
- FR-C2: Toggle to week view and back (`CalendarComponent`); week view lists that week's events
  per day.
- FR-C3: Create / edit / delete a `CalendarEvent` via UI: title, event type (from configured
  types), date, optional time from/to, all-day flag, notes.
- FR-C4: Tapping an event opens `EventDetailComponent`; from there edit and delete.
- FR-C5: Event-type management UI (settings): list, create, rename, recolour, delete custom
  types; the six defaults are seeded and may be edited.
- FR-C6: Creating an event with type **"Prüfung"** launches the Exam assistant (see FR-X6) and
  stores the resulting `Exam.Id` in `CalendarEvent.LinkedExamId`. Tapping a "Prüfung" event
  opens the linked Exam detail view.
- FR-C7: "Export as .ics" action in the calendar UI writes a file (share sheet / file save) of
  all events.

### StudyPlanner (slice 3)

- FR-X1: Create / edit / delete an `Exam`: title, subject (from `SubjectConfig` list), exam date,
  total study minutes, learning goals.
- FR-X2: Recommendation shown live while editing: `goalCount × MinutesPerGoal(subject)` using the
  **persisted** `SubjectConfig` for the selected subject (fallback 15).
- FR-X3: On save, `RecommendedMinutes` and `DailyMinutes` are computed and stored;
  `DailyMinutes = TotalStudyMinutes ÷ max(daysUntilExam, 1)`.
- FR-X4: Subject configuration UI (settings): add / rename / delete subjects, set minutes-per-goal
  per subject; **persisted** to the DB (not just logged).
- FR-X5: Today view: today's study load — the open exams (`DailyMinutes > 0`, `ExamDate >= today`)
  with subject and minutes; total minutes for today.
- FR-X6: Exam assistant (invoked from FR-C6): subject, date (pre-filled from the calendar event),
  learning goals (manual entry and/or Camera OCR — FR-M*), total study time.
- FR-X7: Learning goals belong to an `Exam` (`LearningGoal.ExamId`), have a checkbox and a sort
  order; can be checked off in the Exam detail view.

### Camera / OCR (slice 4)

- FR-M1: Capture a photo with the Android camera (`MediaPicker`).
- FR-M2: Run the image through **Google ML Kit Text Recognition v2** offline, via the
  `Xamarin.Google.MLKit.TextRecognition` NuGet binding, in `MauiCameraService : ICameraService`.
- FR-M3: Each recognised text line becomes one `LearningGoal` (`Text`, `IsChecked = false`,
  `SortOrder` = line index).
- FR-M4: Manual post-editing before saving: add / delete / rename / reorder lines
  (`LearningGoalEditComponent`, already functional).
- FR-M5: Saving links the goals to the current `Exam` and persists them.

### Reminder (slice 5)

- FR-R1: Local notifications on Android via **`Plugin.LocalNotification`** (Notification Channel,
  scheduled + repeating, reschedule after boot).
- FR-R2: Daily study reminder at the configured time (default **15:30**), firing **only** when
  today has study minutes > 0 (sum over open exams with `DailyMinutes > 0` and `ExamDate >= today`).
- FR-R3: Reminder text names the next exam and the minutes, localized — e.g.
  "Lernen für Mathe-Prüfung – 60 Min heute". With several exams: next exam + total minutes today.
- FR-R4: Event reminders for calendar events, N days before (default 1), configurable per event.
- FR-R5: Reminder settings UI persists `ReminderConfig` (default time, event lead days, enabled).
- FR-R6: Cancelling / rescheduling when the underlying exam or event changes or is deleted.

### Widget (slice 6)

- FR-W1: Three Android home-screen widgets (`AppWidgetProvider`): day, week, month.
- FR-W2: Each provider, in `OnUpdate`, opens its **own** `DayDashDbContext` against the known DB
  path (`FileSystem.AppDataDirectory`) and reads real data — no dependency on a live app process
  or DI scope.
- FR-W3: Day widget: today's study plan + next upcoming event. Week widget: current week's events.
  Month widget: mini month grid with markers on days that have events.
- FR-W4: Widgets refresh on their update period and when app data changes (best effort:
  `AppWidgetManager.UpdateAppWidget` triggered from the app after writes).

### Localization (cross-cutting, part of every slice)

- FR-L1: Every user-facing string in a module comes from that module's `.resx`
  (`{Name}Resources.resx` = de-CH default, `{Name}Resources.en.resx` = English). No hardcoded
  UI text, including in the widgets and the new components created earlier.
- FR-L2: Language switch (de-CH / en) in a settings page, stored via `IPreferences`.
- FR-L3: **Live switching without restart** — a `CultureStateService` (analogous to MiniMate)
  updates the culture and re-renders all components immediately.
- FR-L4: Default culture `de-CH`; culture applied on startup from the stored preference.

### Web / MAUI parity (slice 7)

- FR-P1: `DayDash.Web` references all six module RCLs (like `MiniMate.Web`) and hosts the same
  components; routing/pages for Calendar, StudyPlanner, Camera, Settings.
- FR-P2: `NavMenu.razor` / `NavMenu.razor.css`, the global stylesheet
  (`minimate-globals.css` → `daydash-globals.css`), the Bootstrap setup and `culture.js` are
  ported 1:1 from MiniMate into **both** `DayDash.Web` and `DayDash.Maui`; only nav entries and
  texts are adapted to DayDash (Kalender / Lernplan / Kamera / Einstellungen). Logo is a
  placeholder asset; the user supplies the final logo later.
- FR-P3: Components live in the module projects and are consumed unchanged by both hosts.
- FR-P4: In `DayDash.Web`, platform services that cannot run in the browser (camera/OCR,
  notifications, widgets) are replaced by no-op / mock implementations of their interfaces so the
  component previews still render.

## Non-Functional Requirements

- NFR1: **Offline-first** — no network calls anywhere; ML Kit model is on-device.
- NFR2: **Free** — no paid services or SDKs.
- NFR3: **Modular** — each module remains an independent `.csproj`, added/removed via its
  `AddDayDash{Name}()` extension without touching other modules or the host beyond one line.
- NFR4: **Clean Architecture** per module: Domain has no external deps; Application depends on
  Domain + contracts; Infrastructure implements them; UI is Razor components with code-behind
  (`.razor.cs`) and CSS isolation (`.razor.css`).
- NFR5: Language, code identifiers and comments in **English**; UI strings de-CH + en only.
- NFR6: `dotnet build src/DayDash.slnx` → **0 errors** after every vertical slice; warnings kept
  to the pre-existing transitive `NU1903` (SQLite advisory from EF Core Sqlite pin) only.
- NFR7: Automated tests green after every slice.
- NFR8: C# 13 / .NET 10; primary constructors and collection expressions preferred (per CLAUDE.md).
- NFR9: MAUI targets **`net10.0-android` only** (iOS out of scope per product §8).

## Technical Constraints

- **Hosting**: MAUI = Blazor Hybrid (`Microsoft.AspNetCore.Components.WebView.Maui`,
  `AddMauiBlazorWebView`). Web = standalone Blazor WASM (`Microsoft.NET.Sdk.BlazorWebAssembly`).
- **Persistence**: EF Core 10 + `Microsoft.EntityFrameworkCore.Sqlite` on device; EF Core
  InMemory in Web. Migrations checked in. DB path from `FileSystem.AppDataDirectory`.
- **Circular-dependency fix** (Storage ↔ feature modules): resolved via feature-module-owned
  `IEntityTypeConfiguration<T>` contributed to the model at build time; Storage stays free of
  feature-module references. A dedicated `DayDash.Migrations` project (or equivalent) is
  permitted if the plan needs a design-time context.
- **Module ownership**:
  - Calendar: `CalendarEvent`, `EventTypeConfig`, month/week UI, event CRUD UI, event-type
    settings UI, `.ics` export, Exam-assistant launch.
  - StudyPlanner: `Exam`, `LearningGoal`, `SubjectConfig`, exam CRUD UI, subject settings UI,
    today view, recommendation/distribution math, Exam assistant.
  - Camera: `ICameraService` contract + parser + edit UI; ML Kit impl is platform code in
    `DayDash.Maui`.
  - Reminder: `IReminderService` contract + settings UI + `ReminderConfig`; Plugin.LocalNotification
    impl is platform code in `DayDash.Maui`.
  - Widget: `WidgetModuleExtensions` + widget-data query service; `AppWidgetProvider`s are
    platform code in `DayDash.Maui`.
  - Storage: `DayDashDbContext`, `BaseRepository<T>`, migrations, seeder.
- **New platform packages**: `Xamarin.Google.MLKit.TextRecognition`, `Plugin.LocalNotification`
  (both Android-only, added to `DayDash.Maui`).
- **New user-facing strings** in every slice → add keys to both `.resx` files of the owning module.
- **Tests**: `tests/` with xUnit (Application/Domain/Infrastructure logic — calendar grid,
  recommendation/distribution math, `.ics` output, OCR line parser, seeder, repositories over EF
  InMemory) + bUnit (Razor components) + EF Core InMemory. One collecting test project is
  acceptable; unit-case **and** edge-case tests per slice.

## Acceptance Criteria

- [ ] `dotnet build src/DayDash.slnx` → 0 errors after each slice; MAUI `net10.0-android` builds.
- [ ] `dotnet test` → all green after each slice.
- [ ] DB is created via migration on first run and seeded with 6 event types, 5 subjects, 1
      reminder config; re-running does not duplicate seed data.
- [ ] Calendar: user can add an event, see it in month and week view with the correct type
      colour, open it, edit it, delete it.
- [ ] Creating a "Prüfung" event opens the Exam assistant and links the created Exam
      (`LinkedExamId` set); tapping the event opens the Exam detail.
- [ ] Event-type settings: add a custom type with a colour, use it on an event, delete it.
- [ ] `.ics` export produces a file that validates as iCalendar and re-imports into Google
      Calendar with correct dates/titles.
- [ ] StudyPlanner: create an exam with N goals → recommendation = N × subject minutes;
      `DailyMinutes` = total ÷ remaining days; today view shows today's load.
- [ ] Subject settings persist across app restarts.
- [ ] Camera: photograph a printed list → each line appears as an editable goal → save →
      goals are attached to the exam and survive a restart.
- [ ] Reminder: with study minutes today, a notification fires at the configured time with the
      localized next-exam text; with no study load, nothing fires; changing the time in settings
      persists and takes effect.
- [ ] Deleting an exam/event cancels its reminders.
- [ ] Widgets: all three show real data derived from the DB while the app process is not running.
- [ ] Language toggle switches every visible string between de-CH and en **without restart**.
- [ ] No hardcoded user-facing strings remain in any module (including widgets and the
      components added during the earlier green-build pass).
- [ ] `DayDash.Web` renders Calendar / StudyPlanner / Camera / Settings component previews
      against EF InMemory, with the MiniMate NavMenu and global CSS.
- [ ] Code review performed and addressed after each vertical slice.

## Edge Cases & Failure Modes

- Exam date in the past or today → `daysUntilExam` clamped to 1; `DailyMinutes` never divides by
  zero; today view still shows it while `ExamDate >= today`.
- Exam with zero learning goals → recommendation 0; allowed; no reminder text about it.
- Event with `IsAllDay = true` → no time shown; `.ics` uses `VALUE=DATE`.
- Event with only `TimeFrom` (no `TimeTo`) → `.ics` `DTEND` falls back to `TimeFrom` / end of day.
- OCR returns empty / whitespace (bad photo, ML Kit finds nothing) → no goals created, user sees
  a localized "nothing recognised" message; never an unhandled exception.
- Camera permission denied or `MediaPicker` capture cancelled → return to the edit screen, no
  crash, localized hint.
- Deleting a subject that exams reference → exams keep their stored `Subject` string; recommendation
  falls back to 15 min/goal.
- Deleting an `Exam` linked from a calendar event → event stays, `LinkedExamId` cleared; deleting
  a "Prüfung" event → prompt whether to also delete the linked exam.
- Reminder time changed / exam rescheduled / exam deleted → previously scheduled notifications are
  cancelled and re-scheduled; no orphan notifications.
- App has never been opened but a widget is added → widget opens its own context; if the DB file
  does not exist yet it shows a localized empty state, not an error.
- Two widgets / the app open the SQLite file at once → reads only in the widget; rely on SQLite
  shared-cache/WAL; a locked-DB read fails silently to the empty state.
- Language switched while a form is half-filled → field values preserved, only labels re-render.
- `DayDash.Web` (InMemory) restart → data resets; this is expected and acceptable for the preview host.
- Migration fails on an old DB → surface a clear error at startup (log + on-screen), do not delete
  user data silently.

## Out of Scope

- iOS / macOS / Windows heads (Android only).
- Cloud sync, Google/Apple Calendar two-way integration (`.ics` export only).
- Multi-user / family profiles, any login.
- Languages beyond de-CH and en.
- Gamification / points / rewards.
- Push notifications from a server (local notifications only).
- Offline ML model management UI (ML Kit bundles its own model).
- Real persistence in `DayDash.Web` (InMemory preview only).
- Widget interactivity beyond tapping to open the app (no in-widget editing).
- Theming/dark-mode work beyond porting MiniMate's existing styles; final DayDash logo (user
  provides later — placeholder asset only).

## Open Questions

- Exact mechanism for the Storage circular-dependency fix and whether a `DayDash.Migrations`
  project is added — deferred to the plan (user chose "minimal-invasive if needed").
- Whether tests are one collecting project or one per module — plan decides; requirement is only
  xUnit + bUnit + EF InMemory with unit + edge cases per slice.
- Widget refresh trigger from the app after writes — best-effort; exact hook chosen during the
  Widget slice.
- `.ics` export delivery on Android (share sheet vs. save-to-Downloads) — plan/impl decides;
  must produce a user-retrievable file.
