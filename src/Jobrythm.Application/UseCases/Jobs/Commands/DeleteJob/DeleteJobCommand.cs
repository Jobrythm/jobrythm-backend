using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Jobs.Commands.DeleteJob;

public record DeleteJobCommand(Guid Id) : IRequest;

public class DeleteJobCommandHandler(
    IJobRepository jobRepository,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteJobCommand>
{
    public async Task Handle(DeleteJobCommand request, CancellationToken ct)
    {
        var job = await jobRepository.GetByIdAsync(request.Id, ct);
        if (job == null) throw new NotFoundException(nameof(job), request.Id);
        if (job.UserId != currentUserService.UserId) throw new ForbiddenException();

        jobRepository.Remove(job);
        await jobRepository.SaveChangesAsync(ct);
    }
}
