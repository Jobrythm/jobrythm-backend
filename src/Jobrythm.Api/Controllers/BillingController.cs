using Jobrythm.Application.DTOs;
using Jobrythm.Application.UseCases.Billing.Commands.CreateCheckoutSession;
using Jobrythm.Application.UseCases.Billing.Commands.CreatePortalSession;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/billing")]
[Authorize]
public class BillingController(IMediator mediator) : ControllerBase
{
    [HttpPost("checkout")]
    public async Task<ActionResult<StripeSessionDto>> CreateCheckout(CreateCheckoutSessionCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }

    [HttpPost("portal")]
    public async Task<ActionResult<StripeSessionDto>> CreatePortal(CreatePortalSessionCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }
}
