using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Billing.Commands.HandleStripeWebhook;

public record HandleStripeWebhookCommand(string Json, string Signature) : IRequest;

public class HandleStripeWebhookCommandHandler(IStripeService stripeService) : IRequestHandler<HandleStripeWebhookCommand>
{
    public async Task Handle(HandleStripeWebhookCommand request, CancellationToken ct)
    {
        await stripeService.HandleWebhookAsync(request.Json, request.Signature, ct);
    }
}