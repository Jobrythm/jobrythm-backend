using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Jobs.Commands.UpdateJob;

public class UpdateJobCommand : IRequest<JobDto>
{
    public Guid Id { get; set; }
    public Guid ClientId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}

public class UpdateJobCommandHandler(
    IJobRepository jobRepository,
    IClientRepository clientRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateJobCommand, JobDto>
{
    public async Task<JobDto> Handle(UpdateJobCommand request, CancellationToken ct)
    {
        var job = await jobRepository.GetByIdAsync(request.Id, ct);
        if (job == null) throw new NotFoundException(nameof(job), request.Id);
        if (job.UserId != currentUserService.UserId) throw new ForbiddenException();

        var client = await clientRepository.GetByIdAsync(request.ClientId, ct);
        if (client == null) throw new NotFoundException(nameof(client), request.ClientId);
        if (client.UserId != currentUserService.UserId) throw new ForbiddenException();

        job.ClientId = request.ClientId;
        job.Title = request.Title;
        job.Description = request.Description;
        job.StartDate = request.StartDate;
        job.EndDate = request.EndDate;

        await jobRepository.SaveChangesAsync(ct);

        return mapper.Map<JobDto>(job);
    }
}
