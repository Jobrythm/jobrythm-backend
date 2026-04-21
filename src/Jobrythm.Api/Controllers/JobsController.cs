using Jobrythm.Application.DTOs;
using Jobrythm.Application.UseCases.Jobs.Commands.CreateJob;
using Jobrythm.Application.UseCases.Jobs.Commands.DeleteJob;
using Jobrythm.Application.UseCases.Jobs.Commands.UpdateJob;
using Jobrythm.Application.UseCases.Jobs.Commands.UpdateStatus;
using Jobrythm.Application.UseCases.Jobs.Queries.GetJobById;
using Jobrythm.Application.UseCases.Jobs.Queries.GetJobs;
using Jobrythm.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/jobs")]
[Authorize]
public class JobsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<JobDto>>> GetJobs(
        [FromQuery] JobStatus? status, 
        [FromQuery] string? search, 
        [FromQuery] int page = 1, 
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        return await mediator.Send(new GetJobsQuery(search, page, pageSize, status), ct);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<JobDto>> GetJob(Guid id, CancellationToken ct)
    {
        return await mediator.Send(new GetJobByIdQuery(id), ct);
    }

    [HttpPost]
    public async Task<ActionResult<JobDto>> Create(CreateJobCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return CreatedAtAction(nameof(GetJob), new { id = result.Id }, result);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateJobCommand command, CancellationToken ct)
    {
        if (id != command.Id) return BadRequest();
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, UpdateStatusRequest request, CancellationToken ct)
    {
        await mediator.Send(new UpdateJobStatusCommand(id, request.Status), ct);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await mediator.Send(new DeleteJobCommand(id), ct);
        return NoContent();
    }
}

public class UpdateStatusRequest
{
    public JobStatus Status { get; set; }
}
