using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class CalendarWeekView
{
    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;

    private DateTime CurrentWeekStart { get; set; } = StartOfWeek(DateTime.Today, DayOfWeek.Monday);

    private static DateTime StartOfWeek(DateTime date, DayOfWeek startOfWeek)
    {
        var diff = (7 + (date.DayOfWeek - startOfWeek)) % 7;
        return date.AddDays(-diff).Date;
    }
    private DateTime CurrentWeekEnd => CurrentWeekStart.AddDays(6);
    private List<DayViewModel> WeekDays { get; set; } = new();

    protected override void OnInitialized()
    {
        BuildWeekDays();
    }

    private void BuildWeekDays()
    {
        WeekDays.Clear();
        for (var i = 0; i < 7; i++)
        {
            var date = CurrentWeekStart.AddDays(i);
            WeekDays.Add(new DayViewModel { Date = date });
        }
    }

    private void PreviousWeek()
    {
        CurrentWeekStart = CurrentWeekStart.AddDays(-7);
        BuildWeekDays();
    }

    private void NextWeek()
    {
        CurrentWeekStart = CurrentWeekStart.AddDays(7);
        BuildWeekDays();
    }

    private void ShowEventDetails(CalendarEvent calendarEvent)
    {
        // Logic to display event details (e.g., open a modal or navigate to a detail page)
    }

    private class DayViewModel
    {
        public DateTime Date { get; set; }
        public List<CalendarEvent> Events { get; set; } = new();
    }
}