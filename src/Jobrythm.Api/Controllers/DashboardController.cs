using Jobrythm.Application.DTOs;
using Jobrythm.Application.UseCases.Dashboard.Queries.GetDashboardStats;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize]
public class DashboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<DashboardStatsDto>> GetStats(CancellationToken ct)
    {
        return await mediator.Send(new GetDashboardStatsQuery(), ct);
    }
}
