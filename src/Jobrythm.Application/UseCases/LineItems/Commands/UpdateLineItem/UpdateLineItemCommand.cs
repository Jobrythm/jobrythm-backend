using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.LineItems.Commands.UpdateLineItem;

public class UpdateLineItemCommand : IRequest<LineItemDto>
{
    public Guid Id { get; set; }
    public Guid JobId { get; set; }
    public string Description { get; set; } = string.Empty;
    public Jobrythm.Domain.Enums.LineItemCategory Category { get; set; }
    public decimal Quantity { get; set; }
    public long UnitCost { get; set; }
    public long UnitPrice { get; set; }
}

public class UpdateLineItemCommandHandler(
    ILineItemRepository lineItemRepository,
    IJobRepository jobRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateLineItemCommand, LineItemDto>
{
    public async Task<LineItemDto> Handle(UpdateLineItemCommand request, CancellationToken ct)
    {
        var lineItem = await lineItemRepository.GetByIdAsync(request.Id, ct);
        if (lineItem == null) throw new NotFoundException(nameof(lineItem), request.Id);

        var job = await jobRepository.GetByIdAsync(lineItem.JobId, ct);
        if (job == null) throw new NotFoundException(nameof(job), lineItem.JobId);
        if (job.UserId != currentUserService.UserId) throw new ForbiddenException();

        lineItem.Description = request.Description;
        lineItem.Category = request.Category;
        lineItem.Quantity = request.Quantity;
        lineItem.UnitCost = request.UnitCost;
        lineItem.UnitPrice = request.UnitPrice;

        await lineItemRepository.SaveChangesAsync(ct);

        return mapper.Map<LineItemDto>(lineItem);
    }
}
