using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Interfaces;

public interface ILineItemRepository
{
    Task<LineItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(LineItem lineItem, CancellationToken cancellationToken = default);
    void Remove(LineItem lineItem);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
