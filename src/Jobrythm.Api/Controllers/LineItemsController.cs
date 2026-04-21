using Jobrythm.Application.UseCases.LineItems.Commands.CreateLineItem;
using Jobrythm.Application.UseCases.LineItems.Commands.DeleteLineItem;
using Jobrythm.Application.UseCases.LineItems.Commands.UpdateLineItem;
using Jobrythm.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/jobs/{jobId:guid}/lineitems")]
[Authorize]
public class LineItemsController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<LineItemDto>> Create(Guid jobId, CreateLineItemCommand command, CancellationToken ct)
    {
        if (jobId != command.JobId) return BadRequest();
        return await mediator.Send(command, ct);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<LineItemDto>> Update(Guid jobId, Guid id, UpdateLineItemCommand command, CancellationToken ct)
    {
        if (id != command.Id || jobId != command.JobId) return BadRequest();
        return await mediator.Send(command, ct);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid jobId, Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteLineItemCommand(jobId, id), ct);
        return NoContent();
    }
}
