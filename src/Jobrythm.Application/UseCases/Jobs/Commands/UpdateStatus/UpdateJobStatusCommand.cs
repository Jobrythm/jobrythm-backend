using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Enums;
using MediatR;

namespace Jobrythm.Application.UseCases.Jobs.Commands.UpdateStatus;

public record UpdateJobStatusCommand(Guid Id, JobStatus Status) : IRequest;

public class UpdateJobStatusCommandHandler(
    IJobRepository jobRepository,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateJobStatusCommand>
{
    public async Task Handle(UpdateJobStatusCommand request, CancellationToken ct)
    {
        var job = await jobRepository.GetByIdAsync(request.Id, ct);
        if (job == null) throw new NotFoundException(nameof(job), request.Id);

        if (job.UserId != currentUserService.UserId)
            throw new ForbiddenException();

        job.Status = request.Status;
        await jobRepository.SaveChangesAsync(ct);
    }
}
