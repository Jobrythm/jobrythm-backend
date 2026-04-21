using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly JobrythmDbContext _context;

    public InvoiceRepository(JobrythmDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Invoices
            .Include(i => i.Job)
            .ThenInclude(j => j.Client)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Invoice>> GetPagedAsync(
        string userId, 
        InvoiceStatus? status, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        var query = _context.Invoices.AsNoTracking()
            .Where(i => i.Job.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(i => i.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(i => i.Job)
            .ThenInclude(j => j.Client)
            .OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Invoice>(items, totalCount, page, pageSize);
    }

    public async Task<Invoice?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken)
    {
        return await _context.Invoices
            .FirstOrDefaultAsync(i => i.JobId == jobId, cancellationToken);
    }

    public async Task AddAsync(Invoice invoice, CancellationToken cancellationToken)
    {
        await _context.Invoices.AddAsync(invoice, cancellationToken);
    }

    public void Remove(Invoice invoice)
    {
        _context.Invoices.Remove(invoice);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
