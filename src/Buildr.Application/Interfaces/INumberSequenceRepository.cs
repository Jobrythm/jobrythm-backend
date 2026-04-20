using Buildr.Domain.Entities;

namespace Buildr.Application.Interfaces;

public interface INumberSequenceRepository
{
    Task<NumberSequence?> GetByUserIdAndPrefixAsync(string userId, string prefix, CancellationToken cancellationToken = default);
    Task<NumberSequence> AddAsync(NumberSequence sequence, CancellationToken cancellationToken = default);
    Task UpdateAsync(NumberSequence sequence, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atomically increments the sequence for the given user and prefix and returns the new number.
    /// </summary>
    Task<int> GetNextNumberAsync(string userId, string prefix, CancellationToken cancellationToken = default);
}
