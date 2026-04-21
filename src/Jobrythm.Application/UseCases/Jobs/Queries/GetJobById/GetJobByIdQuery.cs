using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Jobs.Queries.GetJobById;

public record GetJobByIdQuery(Guid Id) : IRequest<JobDto>;

public class GetJobByIdQueryHandler(
    IJobRepository jobRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetJobByIdQuery, JobDto>
{
    public async Task<JobDto> Handle(GetJobByIdQuery request, CancellationToken ct)
    {
        var job = await jobRepository.GetByIdWithDetailsAsync(request.Id, ct);
        
        if (job == null) throw new NotFoundException(nameof(job), request.Id);
        if (job.UserId != currentUserService.UserId) throw new ForbiddenException();

        return mapper.Map<JobDto>(job);
    }
}
