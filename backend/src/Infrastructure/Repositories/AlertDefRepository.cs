using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class AlertDefRepository : IAlertDefRepository
{
    private readonly AppDbContext _context;

    public AlertDefRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<AlertDef?> GetByIdAsync(Guid id)
    {
        return await _context.AlertDefs.FindAsync(id);
    }

    public async Task<(List<AlertDef> Items, int Total)> GetByUserIdAsync(Guid userId, int page, int limit)
    {
        var query = _context.AlertDefs.Where(a => a.UserId == userId);
        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(a => a.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return (items, total);
    }

    public async Task<AlertDef> CreateAsync(AlertDef alertDef)
    {
        _context.AlertDefs.Add(alertDef);
        await _context.SaveChangesAsync();
        return alertDef;
    }

    public async Task<AlertDef> UpdateAsync(AlertDef alertDef)
    {
        _context.AlertDefs.Update(alertDef);
        await _context.SaveChangesAsync();
        return alertDef;
    }

    public async Task DeleteAsync(AlertDef alertDef)
    {
        _context.AlertDefs.Remove(alertDef);
        await _context.SaveChangesAsync();
    }
}
