using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Infrastructure.Persistence.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly JobrythmDbContext _context;

    public ClientRepository(JobrythmDbContext context)
    {
        _context = context;
    }

    public async Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Clients.FindAsync([id], cancellationToken);
    }

    public async Task<PagedResult<Client>> GetPagedAsync(
        string userId, 
        string? search, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        var query = _context.Clients.AsNoTracking().Where(c => c.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => EF.Functions.ILike(c.Name, $"%{search}%") || 
                                    EF.Functions.ILike(c.Email ?? "", $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Client>(items, totalCount, page, pageSize);
    }

    public async Task<int> GetJobCountAsync(Guid clientId, CancellationToken cancellationToken)
    {
        return await _context.Jobs.CountAsync(j => j.ClientId == clientId, cancellationToken);
    }

    public async Task<long> GetTotalRevenueAsync(Guid clientId, CancellationToken cancellationToken)
    {
        return await _context.Invoices
            .Where(i => i.Job.ClientId == clientId && i.Status == InvoiceStatus.Paid)
            .SumAsync(i => (long)i.Job.LineItems.Sum(li => (double)li.Quantity * li.UnitPrice), cancellationToken);
    }

    public async Task AddAsync(Client client, CancellationToken cancellationToken)
    {
        await _context.Clients.AddAsync(client, cancellationToken);
    }

    public void Remove(Client client)
    {
        _context.Clients.Remove(client);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
