using Jobrythm.Application.DTOs;
using Jobrythm.Application.UseCases.Invoices.Commands.CreateInvoice;
using Jobrythm.Application.UseCases.Invoices.Commands.GenerateInvoicePdf;
using Jobrythm.Application.UseCases.Invoices.Commands.MarkPaid;
using Jobrythm.Application.UseCases.Invoices.Commands.SendInvoice;
using Jobrythm.Application.UseCases.Invoices.Commands.UpdateInvoice;
using Jobrythm.Application.UseCases.Invoices.Queries.GetInvoiceById;
using Jobrythm.Application.UseCases.Invoices.Queries.GetInvoices;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/invoices")]
[Authorize]
public class InvoicesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<InvoiceDto>>> GetInvoices(
        [FromQuery] string? search, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        return await mediator.Send(new GetInvoicesQuery(search, page, pageSize), ct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InvoiceDto>> GetInvoice(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new GetInvoiceByIdQuery(id), ct);
    }

    [HttpPost("/api/jobs/{jobId:guid}/invoices")]
    public async Task<ActionResult<InvoiceDto>> Create(Guid jobId, CreateInvoiceCommand command, CancellationToken ct)
    {
        if (jobId != command.JobId) return BadRequest();
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetInvoice), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateInvoiceCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/paid")]
    public async Task<ActionResult<InvoiceDto>> MarkPaid(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new MarkInvoicePaidCommand(id), ct);
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GetPdf(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GenerateInvoicePdfCommand(id), ct);
        return File(result.Content, "application/pdf", result.FileName);
    }

    [HttpPost("{id:guid}/send")]
    public async Task<IActionResult> Send(Guid id, CancellationToken ct)
    {
        await mediator.Send(new SendInvoiceCommand(id), ct);
        return NoContent();
    }
}
