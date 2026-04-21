using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Jobs.Queries.GetJobs;

public record GetJobsQuery(string? Search, int Page, int PageSize, Domain.Enums.JobStatus? Status = null) : IRequest<PagedResult<JobDto>>;

public class GetJobsQueryHandler(
    IJobRepository jobRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetJobsQuery, PagedResult<JobDto>>
{
    public async Task<PagedResult<JobDto>> Handle(GetJobsQuery request, CancellationToken ct)
    {
        var result = await jobRepository.GetPagedAsync(
            currentUserService.UserId!, 
            request.Status, 
            request.Search, 
            request.Page, 
            request.PageSize, 
            ct);

        var dtos = mapper.Map<IReadOnlyList<JobDto>>(result.Items);
        return new PagedResult<JobDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}
