using Jobrythm.Application.DTOs;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;

namespace Jobrythm.Application.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Invoice>> GetPagedAsync(string userId, InvoiceStatus? status, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Invoice?> GetByJobIdAsync(Guid jobId, CancellationToken cancellationToken = default);
    Task AddAsync(Invoice invoice, CancellationToken cancellationToken = default);
    void Remove(Invoice invoice);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
