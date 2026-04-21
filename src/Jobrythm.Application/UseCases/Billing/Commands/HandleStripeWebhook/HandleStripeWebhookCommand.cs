using MediatR;

namespace Jobrythm.Application.UseCases.Billing.Commands.HandleStripeWebhook;

public record HandleStripeWebhookCommand(string Json, string Signature) : IRequest;

public class HandleStripeWebhookCommandHandler : IRequestHandler<HandleStripeWebhookCommand>
{
    public Task Handle(HandleStripeWebhookCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}