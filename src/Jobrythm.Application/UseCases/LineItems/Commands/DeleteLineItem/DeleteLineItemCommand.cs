using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.LineItems.Commands.DeleteLineItem;

public record DeleteLineItemCommand(Guid JobId, Guid Id) : IRequest;

public class DeleteLineItemCommandHandler(
    ILineItemRepository lineItemRepository,
    IJobRepository jobRepository,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteLineItemCommand>
{
    public async Task Handle(DeleteLineItemCommand request, CancellationToken ct)
    {
        var lineItem = await lineItemRepository.GetByIdAsync(request.Id, ct);
        if (lineItem == null) throw new NotFoundException(nameof(lineItem), request.Id);

        var job = await jobRepository.GetByIdAsync(lineItem.JobId, ct);
        if (job == null) throw new NotFoundException(nameof(job), lineItem.JobId);
        if (job.UserId != currentUserService.UserId) throw new ForbiddenException();

        lineItemRepository.Remove(lineItem);
        await lineItemRepository.SaveChangesAsync(ct);
    }
}
