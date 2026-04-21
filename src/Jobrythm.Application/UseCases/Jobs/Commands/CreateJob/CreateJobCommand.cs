using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Jobs.Commands.CreateJob;

public class CreateJobCommand : IRequest<JobDto>
{
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class CreateJobCommandHandler(
    IJobRepository jobRepository,
    IClientRepository clientRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateJobCommand, JobDto>
{
    public async Task<JobDto> Handle(CreateJobCommand request, CancellationToken ct)
    {
        var client = await clientRepository.GetByIdAsync(request.ClientId, ct);
        if (client == null) throw new NotFoundException(nameof(client), request.ClientId);

        if (client.UserId != currentUserService.UserId)
            throw new ForbiddenException();

        var job = new Domain.Entities.Job
        {
            UserId = currentUserService.UserId!,
            ClientId = request.ClientId,
            Title = request.Title,
            Description = request.Description,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            Status = Domain.Enums.JobStatus.Draft
        };

        await jobRepository.AddAsync(job, ct);
        // await jobRepository.SaveChangesAsync(ct); // Repository should handle save changes or unit of work

        return mapper.Map<JobDto>(job);
    }
}
