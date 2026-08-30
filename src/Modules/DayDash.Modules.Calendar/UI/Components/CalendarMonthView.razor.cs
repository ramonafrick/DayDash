using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Calendar.Resources;

namespace DayDash.Modules.Calendar.UI.Components;

public partial class CalendarMonthView
{
    [Inject] private IStringLocalizer<CalendarResources> Loc { get; set; } = default!;

    private DateTime CurrentMonth { get; set; } = DateTime.Today;
    private List<List<DayViewModel>> MonthGrid { get; set; } = new();

    protected override void OnInitialized()
    {
        BuildMonthGrid();
    }

    private void BuildMonthGrid()
    {
        // Logic to build the month grid with weeks and days
    }

    private void PreviousMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(-1);
        BuildMonthGrid();
    }

    private void NextMonth()
    {
        CurrentMonth = CurrentMonth.AddMonths(1);
        BuildMonthGrid();
    }

    private void SelectDay(DateTime date)
    {
        // Logic to handle day selection
    }

    private void ShowEventDetails(DateTime date)
    {
        var events = MonthGrid.SelectMany(week => week)
                          .FirstOrDefault(day => day.Date == date)?.EventTypes;
        if (events != null && events.Any())
        {
            // Logic to display event details (e.g., open a modal or navigate to a detail page)
        }
    }

    private class DayViewModel
    {
        public DateTime Date { get; set; }
        public bool IsToday => Date.Date == DateTime.Today;
        public List<EventTypeConfig> EventTypes { get; set; } = new();
    }
}