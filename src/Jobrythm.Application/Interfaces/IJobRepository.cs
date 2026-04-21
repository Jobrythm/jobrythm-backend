using Jobrythm.Application.DTOs;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;

namespace Jobrythm.Application.Interfaces;

public interface IJobRepository
{
    Task<Job?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Job?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<Job>> GetPagedAsync(string userId, JobStatus? status, string? search, int page, int pageSize, CancellationToken cancellationToken = default);
    Task AddAsync(Job job, CancellationToken cancellationToken = default);
    void Remove(Job job);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
