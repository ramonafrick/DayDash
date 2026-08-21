# DayDash – Requirements

> Kinder-Kalender-App mit Lernplaner, Erinnerungen und Widget-Funktion  
> Technologie: .NET MAUI + Blazor Hybrid | Modulare Clean Architecture

---

## 1. Projektziel

DayDash ist eine lokale, datenschutzfreundliche Kalender- und Lernplan-App für Kinder im Alter von 8–14 Jahren. Sie hilft Kindern, Prüfungen zu planen, Lernziele zu verfolgen und den Schulalltag zu organisieren – ohne Cloud-Zwang und ohne Kosten.

---

## 2. Zielgruppe & Geräte

| Eigenschaft | Wert |
|---|---|
| Altersgruppe | 8–14 Jahre |
| Geräte | Android (Smartphone & Tablet) |
| Nutzermodell | Single-User (kein Login, keine Cloud) |
| Datenspeicherung | Lokal (SQLite via EF Core) |
| App-Sprache | Deutsch (CH) Standard, Englisch umschaltbar |

---

## 3. Mehrsprachigkeit

Die App unterstützt zwei Sprachen, umschaltbar in den Einstellungen:

| Sprache | Kürzel | Standard |
|---|---|---|
| Deutsch (Schweiz) | `de-CH` | ✅ |
| Englisch | `en` | – |

**Konzept (analog MiniMate):**
- Jedes Modul hat eigene `.resx`-Dateien in `Resources/`
- `{Name}Resources.resx` = Deutsch CH (Default)
- `{Name}Resources.en.resx` = Englisch
- Sprachwahl wird lokal via `IPreferences` gespeichert
- Kein hardcodierter Text in Razor-Komponenten oder Services
- Alle Code-Bezeichner, Kommentare und Instruction Files sind **ausschliesslich auf Englisch**

---

## 4. Module & Architektur

Die App folgt dem gleichen Muster wie **MiniMate** – pro Modul eine eigene Clean Architecture, die unabhängig hinzugefügt oder entfernt werden kann.

```
src/
├── DayDash.sln
├── DayDash.Maui/               # MAUI Host (Blazor Hybrid)
├── DayDash.Web/                # Blazor WASM (optional / Browser-Dev)
└── Modules/
    ├── DayDash.Modules.Calendar/
    ├── DayDash.Modules.Camera/
    ├── DayDash.Modules.StudyPlanner/
    ├── DayDash.Modules.Reminder/
    ├── DayDash.Modules.Widget/
    └── DayDash.Modules.Storage/
```

Jedes Modul enthält:
```
DayDash.Modules.{Name}/
├── Application/
│   ├── Contracts/       # Interfaces (z.B. ICalendarService)
│   ├── Models/          # DTOs / Request-Response Modelle
│   └── Services/        # Use-Case Implementierungen
├── Domain/              # Entities, Enums, Value Objects
├── Infrastructure/      # DB, externe Libs, Platform-spezifisch
├── Resources/           # Lokalisierung (.resx) de-CH + en
├── UI/
│   └── Components/      # Razor-Komponenten (.razor)
├── {Name}ModuleExtensions.cs   # DI-Registration
└── DayDash.Modules.{Name}.csproj
```

---

## 5. Module im Detail

### 5.1 Modul: Calendar

**Zweck:** Hauptkalender mit Monats- und Wochenansicht

**Features:**
- Monatsansicht (Standard), umschaltbar zur Wochenansicht
- Tap auf Termin → Detailansicht
- Event-Typen (Standard, konfigurierbar):
  - `Prüfung` – mit Lernplan-Verknüpfung
  - `Hausaufgaben` (Kürzel: HA)
  - `Schulferien`
  - `Geburtstag`
  - `Abmachung`
  - `Sonstiges` – frei deklarierbar
- Eigene Event-Typen in Einstellungen erstellen
- Farbcodierung pro Event-Typ
- Export aller Events als `.ics` (iCalendar) für Google/Apple Calendar

**Entitäten:**
```
CalendarEvent
├── Id (Guid)
├── Title (string)
├── EventType (string – konfigurierbar)
├── Date (DateOnly)
├── TimeFrom (TimeOnly?)
├── TimeTo (TimeOnly?)
├── Notes (string?)
├── IsAllDay (bool)
└── LinkedExamId (Guid?)
```

---

### 5.2 Modul: Camera (OCR)

**Zweck:** Lernziele per Foto erfassen und als Text speichern

**Technologie:** Google ML Kit Text Recognition v2 – vollständig **offline**, gratis, ~3–5 MB

**Ablauf:**
1. Kind fotografiert die Lernziel-Liste der Lehrerin
2. ML Kit erkennt Text 1:1 (inkl. Zeilenstruktur)
3. Jede Zeile → eigenständiges Lernziel mit Checkbox
4. Manuelle Nachbearbeitung möglich (Zeilen hinzufügen / löschen / umbenennen)
5. Speichern verknüpft Lernziele mit der Prüfung

**Entitäten:**
```
LearningGoal
├── Id (Guid)
├── ExamId (Guid)
├── Text (string)
├── IsChecked (bool)
└── SortOrder (int)
```

---

### 5.3 Modul: StudyPlanner (Lernplan)

**Zweck:** Lernzeit berechnen und auf verbleibende Tage verteilen

**Features:**
- Prüfungserstellung mit Fach, Datum, Lernzielen, Gesamtlernzeit
- Empfehlung: `Anzahl Lernziele × Minuten/Lernziel = Empfehlung`
- Standard: 15 Min/Lernziel – **pro Fach konfigurierbar**
- Verteilung: `Gesamtzeit ÷ verbleibende Tage = Lernzeit/Tag`
- Tagesansicht mit heutigem Lernpensum

**Konfigurierbare Fächer (Standard, erweiterbar):**
- Mathematik, Deutsch, NMG, Englisch, Französisch
- Einstellungsmaske: Fächer hinzufügen / umbenennen / löschen
- Min/Lernziel pro Fach konfigurieren

**Entitäten:**
```
Exam
├── Id (Guid)
├── Title (string)
├── Subject (string)
├── ExamDate (DateOnly)
├── TotalStudyMinutes (int)
├── RecommendedMinutes (int)
├── DailyMinutes (int)
└── LearningGoals (List<LearningGoal>)

SubjectConfig
├── Id (Guid)
├── Name (string)
└── MinutesPerGoal (int)    # Default: 15
```

---

### 5.4 Modul: Reminder

**Zweck:** Push-Notifications für Lernen und Termine

**Features:**
- Täglicher Lern-Reminder (Standard: **15:30 Uhr**)
- Inhalt: „Lernen für [Prüfung xyz] – 1h heute"
- Nur aktiv wenn heute Lernzeit eingeplant ist
- Termin-Reminder für Kalender-Events
- Individuell einstellbar pro Prüfung / Event
- Standard-Push-Notification (Android Notification Channel)
- Anzeige auf Sperrbildschirm

**Konfiguration:**
- Standard-Uhrzeit Lern-Reminder (änderbar in Einstellungen)
- Reminder-Vorlaufzeit für Events in Tagen

---

### 5.5 Modul: Widget

**Zweck:** Android Home-Screen Widget

**Features (3 Ansichten):**
- **Tagesübersicht:** Heutiger Lernplan + nächster Termin
- **Wochenübersicht:** Events der laufenden Woche
- **Monatsübersicht:** Mini-Kalender mit markierten Terminen

**Technologie:** Android `AppWidgetProvider` (MAUI Plattform-spezifisch)

---

### 5.6 Modul: Storage

**Zweck:** Lokale Datenpersistenz und Export

**Features:**
- SQLite-Datenbank via EF Core
- Repository-Pattern pro Entität
- Migrations-Support
- Export als `.ics` (iCalendar-Format)
- Kein Cloud-Sync (Architektur lässt spätere Erweiterung zu)

---

## 6. Nicht-funktionale Anforderungen

| Anforderung | Beschreibung |
|---|---|
| Offline-First | Alles ohne Internet nutzbar |
| Datenschutz | Keine externen API-Calls |
| Kostenlos | Kein Abo, keine kostenpflichtigen Dienste |
| Erweiterbar | Neue Module ohne Änderungen am Kern |
| Exportierbar | `.ics`-Export für spätere Migration |
| Mehrsprachig | `de-CH` / `en` via `.resx` wie MiniMate |

---

## 7. Technologie-Stack

| Bereich | Technologie |
|---|---|
| Framework | .NET 10 / MAUI + Blazor Hybrid |
| Sprache | C# 13 |
| UI | Blazor Razor Components |
| Datenbank | SQLite + EF Core |
| OCR | Google ML Kit Text Recognition v2 (offline) |
| Notifications | MAUI / Android Notification Channels |
| Widget | Android AppWidgetProvider |
| Lokalisierung | .resx + IStringLocalizer (analog MiniMate) |
| Export | iCalendar (.ics) |
| DI | Microsoft.Extensions.DependencyInjection |
| Architektur | Modulare Clean Architecture (analog MiniMate) |

---

## 8. Out of Scope (MVP)

- Cloud-Sync / Google Calendar-Anbindung (Export bereits vorbereitet)
- iOS-Support (Architektur erlaubt spätere Erweiterung)
- Multi-User / Familienprofile
- Weitere Sprachen (DE/EN reicht für MVP)
- Gamification / Punkte-System
