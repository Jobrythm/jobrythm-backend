using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task<Invoice> AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task UpdateAsync(Invoice invoice, CancellationToken cancellationToken = default);
    Task DeleteAsync(Invoice invoice, CancellationToken cancellationToken = default);
}
