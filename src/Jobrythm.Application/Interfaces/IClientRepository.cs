using Jobrythm.Application.DTOs;
using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Interfaces;

public interface IClientRepository
{
    Task<Client?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Client>> GetByUserIdAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Client> AddAsync(Client client, CancellationToken cancellationToken = default);
    Task UpdateAsync(Client client, CancellationToken cancellationToken = default);
    Task DeleteAsync(Client client, CancellationToken cancellationToken = default);
}
