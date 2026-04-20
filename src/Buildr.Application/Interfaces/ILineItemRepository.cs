using Buildr.Domain.Entities;

namespace Buildr.Application.Interfaces;

public interface ILineItemRepository
{
    Task<LineItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<LineItem>> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<LineItem> AddAsync(LineItem lineItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(LineItem lineItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(LineItem lineItem, CancellationToken cancellationToken = default);
}
