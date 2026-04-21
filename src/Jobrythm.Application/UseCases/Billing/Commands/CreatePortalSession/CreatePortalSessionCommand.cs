using Jobrythm.Application.DTOs;
using MediatR;

namespace Jobrythm.Application.UseCases.Billing.Commands.CreatePortalSession;

public record CreatePortalSessionCommand(string ReturnUrl) : IRequest<StripeSessionDto>;

public class CreatePortalSessionCommandHandler : IRequestHandler<CreatePortalSessionCommand, StripeSessionDto>
{
    public Task<StripeSessionDto> Handle(CreatePortalSessionCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}