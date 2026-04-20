using Jobrythm.Application.DTOs;
using Jobrythm.Domain.Entities;

namespace Jobrythm.Application.Interfaces;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Job>> GetByUserIdAsync(string userId, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
    Task<Job> AddAsync(Job job, CancellationToken cancellationToken = default);
    Task UpdateAsync(Job job, CancellationToken cancellationToken = default);
    Task DeleteAsync(Job job, CancellationToken cancellationToken = default);
}
