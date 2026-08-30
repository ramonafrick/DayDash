using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Calendar.Domain;
using DayDash.Modules.Storage.Application.Contracts;

namespace DayDash.Modules.Calendar.Application.Contracts;

public interface ICalendarRepository : IRepository<CalendarEvent>
{
    Task<IReadOnlyList<CalendarEvent>> GetByDateAsync(DateOnly date, CancellationToken ct = default);
    Task<IReadOnlyList<CalendarEvent>> GetByMonthAsync(int year, int month, CancellationToken ct = default);
    Task<IReadOnlyList<CalendarEvent>> GetByDateRangeAsync(DateOnly from, DateOnly to, CancellationToken ct = default);
}