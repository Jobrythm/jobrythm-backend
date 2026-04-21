using Jobrythm.Application.DTOs;
using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Client>> GetPagedAsync(string userId, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> GetJobCountAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task<long> GetTotalRevenueAsync(Guid clientId, CancellationToken cancellationToken = default);
    Task AddAsync(Client client, CancellationToken cancellationToken = default);
    void Remove(Client client);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
