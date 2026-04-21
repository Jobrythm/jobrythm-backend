using Jobrythm.Application.DTOs;
using Jobrythm.Application.UseCases.Quotes.Commands.CreateQuote;
using Jobrythm.Application.UseCases.Quotes.Commands.GenerateQuotePdf;
using Jobrythm.Application.UseCases.Quotes.Commands.SendQuote;
using Jobrythm.Application.UseCases.Quotes.Commands.UpdateQuote;
using Jobrythm.Application.UseCases.Quotes.Queries.GetQuoteById;
using Jobrythm.Application.UseCases.Quotes.Queries.GetQuotes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/quotes")]
[Authorize]
public class QuotesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<QuoteDto>>> GetQuotes(
        [FromQuery] string? search, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        return await mediator.Send(new GetQuotesQuery(search, page, pageSize), ct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<QuoteDto>> GetQuote(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new GetQuoteByIdQuery(id), ct);
    }

    [HttpPost("/api/jobs/{jobId:guid}/quotes")]
    public async Task<ActionResult<QuoteDto>> Create(Guid jobId, CreateQuoteCommand command, CancellationToken ct)
    {
        if (jobId != command.JobId) return BadRequest();
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetQuote), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateQuoteCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpGet("{id:guid}/pdf")]
    public async Task<IActionResult> GetPdf(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GenerateQuotePdfCommand(id), ct);
        return File(result.Content, "application/pdf", result.FileName);
    }

    [HttpPost("{id:guid}/send")]
    public async Task<IActionResult> Send(Guid id, CancellationToken ct)
    {
        await mediator.Send(new SendQuoteCommand(id), ct);
        return NoContent();
    }
}
