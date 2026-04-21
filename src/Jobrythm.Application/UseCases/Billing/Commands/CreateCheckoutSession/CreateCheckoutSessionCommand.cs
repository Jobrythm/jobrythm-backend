using Jobrythm.Application.DTOs;
using MediatR;

namespace Jobrythm.Application.UseCases.Billing.Commands.CreateCheckoutSession;

public record CreateCheckoutSessionCommand(string PriceId, string SuccessUrl, string CancelUrl) : IRequest<StripeSessionDto>;

public class CreateCheckoutSessionCommandHandler : IRequestHandler<CreateCheckoutSessionCommand, StripeSessionDto>
{
    public Task<StripeSessionDto> Handle(CreateCheckoutSessionCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}