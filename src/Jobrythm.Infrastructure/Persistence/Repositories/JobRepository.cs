using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Infrastructure.Persistence.Repositories;

public class JobRepository : IJobRepository
{
    private readonly JobrythmDbContext _context;

    public JobRepository(JobrythmDbContext context)
    {
        _context = context;
    }

    public async Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Jobs.FindAsync([id], cancellationToken);
    }

    public async Task<Job?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Jobs
            .Include(j => j.Client)
            .Include(j => j.LineItems)
            .Include(j => j.Quote)
            .Include(j => j.Invoice)
            .FirstOrDefaultAsync(j => j.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Job>> GetPagedAsync(
        string userId, 
        JobStatus? status, 
        string? search, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        var query = _context.Jobs.AsNoTracking().Where(j => j.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(j => j.Status == status.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(j => EF.Functions.ILike(j.Title, $"%{search}%") || 
                                    EF.Functions.ILike(j.Description ?? "", $"%{search}%"));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(j => j.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Job>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync(Job job, CancellationToken cancellationToken)
    {
        await _context.Jobs.AddAsync(job, cancellationToken);
    }

    public void Remove(Job job)
    {
        _context.Jobs.Remove(job);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
