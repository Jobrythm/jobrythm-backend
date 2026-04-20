using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Interfaces;

public interface IQuoteRepository
{
    Task<Quote?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Quote?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Quote> AddAsync(Quote quote, CancellationToken cancellationToken = default);
    Task UpdateAsync(Quote quote, CancellationToken cancellationToken = default);
    Task DeleteAsync(Quote quote, CancellationToken cancellationToken = default);
}
