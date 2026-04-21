using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Interfaces;

public interface INumberSequenceRepository
{
    Task<NumberSequence?> GetAsync(string userId, string prefix, CancellationToken cancellationToken = default);
    Task AddAsync(NumberSequence sequence, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
