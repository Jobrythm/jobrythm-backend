using Jobrythm.Application.DTOs;
using Jobrythm.Application.UseCases.Clients.Commands.CreateClient;
using Jobrythm.Application.UseCases.Clients.Commands.DeleteClient;
using Jobrythm.Application.UseCases.Clients.Commands.UpdateClient;
using Jobrythm.Application.UseCases.Clients.Queries.GetClientById;
using Jobrythm.Application.UseCases.Clients.Queries.GetClients;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/clients")]
[Authorize]
public class ClientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<ClientDto>>> GetClients(
        [FromQuery] string? search, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        return await mediator.Send(new GetClientsQuery(search, page, pageSize), ct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ClientDto>> GetClient(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new GetClientByIdQuery(id), ct);
    }

    [HttpPost]
    public async Task<ActionResult<ClientDto>> Create(CreateClientCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetClient), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateClientCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteClientCommand(id), ct);
        return NoContent();
    }
}
