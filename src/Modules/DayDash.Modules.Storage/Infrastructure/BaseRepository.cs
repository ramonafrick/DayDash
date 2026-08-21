using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DayDash.Modules.Storage.Application.Contracts;
using Microsoft.EntityFrameworkCore;

namespace DayDash.Modules.Storage.Infrastructure;

public class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly DayDashDbContext _context;

    public BaseRepository(DayDashDbContext context)
    {
        _context = context;
    }

    public async Task<T?> GetByIdAsync(object id, CancellationToken ct = default)
        => await _context.Set<T>().FindAsync(new object[] { id }, ct);

    public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
        => await _context.Set<T>().ToListAsync(ct);

    public async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>().AddAsync(entity, ct);
        await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Update(entity);
        await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(T entity, CancellationToken ct = default)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync(ct);
    }
}