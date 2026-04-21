using Jobrythm.Application.UseCases.Billing.Commands.HandleStripeWebhook;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public class StripeWebhookController(IMediator mediator) : ControllerBase
{
    [HttpPost("stripe")]
    public async Task<IActionResult> HandleStripe(CancellationToken ct)
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync(ct);
        var signature = Request.Headers["Stripe-Signature"];

        if (string.IsNullOrEmpty(signature))
            return BadRequest();

        await mediator.Send(new HandleStripeWebhookCommand(json, signature!), ct);
        return Ok();
    }
}
