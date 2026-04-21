using Jobrythm.Application.DTOs;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;

namespace Jobrythm.Application.Interfaces;

public interface IQuoteRepository
{
    Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Quote>> GetPagedAsync(string userId, QuoteStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Quote?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task AddAsync(Quote quote, CancellationToken cancellationToken = default);
    void Remove(Quote quote);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
