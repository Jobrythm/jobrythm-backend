using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;

namespace Jobrythm.Application.UseCases.LineItems.Commands.CreateLineItem;

public class CreateLineItemCommand : IRequest<LineItemDto>
{
    public Guid JobId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public long UnitCost { get; set; }
    public long UnitPrice { get; set; }
    public Jobrythm.Domain.Enums.LineItemCategory Category { get; set; }
}

public class CreateLineItemCommandHandler(
    IJobRepository jobRepository,
    ILineItemRepository lineItemRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateLineItemCommand, LineItemDto>
{
    public async Task<LineItemDto> Handle(CreateLineItemCommand request, CancellationToken ct)
    {
        var job = await jobRepository.GetByIdAsync(request.JobId, ct);
        if (job == null) throw new NotFoundException(nameof(job), request.JobId);

        if (job.UserId != currentUserService.UserId)
            throw new ForbiddenException();

        var lineItem = new LineItem
        {
            JobId = request.JobId,
            Description = request.Description,
            Quantity = request.Quantity,
            UnitCost = request.UnitCost,
            UnitPrice = request.UnitPrice,
            Category = request.Category
        };

        await lineItemRepository.AddAsync(lineItem, ct);
        await lineItemRepository.SaveChangesAsync(ct);
        return mapper.Map<LineItemDto>(lineItem);
    }
}
