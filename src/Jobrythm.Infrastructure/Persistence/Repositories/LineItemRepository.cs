using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Infrastructure.Persistence.Repositories;

public class LineItemRepository : ILineItemRepository
{
    private readonly JobrythmDbContext _context;

    public LineItemRepository(JobrythmDbContext context)
    {
        _context = context;
    }

    public async Task<LineItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.LineItems.FindAsync([id], cancellationToken);
    }

    public async Task AddAsync(LineItem lineItem, CancellationToken cancellationToken)
    {
        await _context.LineItems.AddAsync(lineItem, cancellationToken);
    }

    public void Remove(LineItem lineItem)
    {
        _context.LineItems.Remove(lineItem);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
