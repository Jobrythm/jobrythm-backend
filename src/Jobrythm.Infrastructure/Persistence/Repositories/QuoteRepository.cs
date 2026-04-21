using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Infrastructure.Persistence.Repositories;

public class QuoteRepository : IQuoteRepository
{
    private readonly JobrythmDbContext _context;

    public QuoteRepository(JobrythmDbContext context)
    {
        _context = context;
    }

    public async Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Quotes
            .Include(q => q.Job)
            .FirstOrDefaultAsync(q => q.Id == id, cancellationToken);
    }

    public async Task<PagedResult<Quote>> GetPagedAsync(
        string userId, 
        QuoteStatus? status, 
        int page, 
        int pageSize, 
        CancellationToken cancellationToken)
    {
        var query = _context.Quotes.AsNoTracking()
            .Where(q => q.Job.UserId == userId);

        if (status.HasValue)
        {
            query = query.Where(q => q.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Include(q => q.Job)
            .OrderByDescending(q => q.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Quote>(items, totalCount, page, pageSize);
    }

    public async Task<Quote?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken)
    {
        return await _context.Quotes
            .FirstOrDefaultAsync(q => q.JobId == jobId, cancellationToken);
    }

    public async Task AddAsync(Quote quote, CancellationToken cancellationToken)
    {
        await _context.Quotes.AddAsync(quote, cancellationToken);
    }

    public void Remove(Quote quote)
    {
        _context.Quotes.Remove(quote);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
