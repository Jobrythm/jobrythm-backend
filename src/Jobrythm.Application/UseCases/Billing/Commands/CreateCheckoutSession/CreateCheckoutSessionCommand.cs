using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Jobrythm.Application.UseCases.Billing.Commands.CreateCheckoutSession;

public record CreateCheckoutSessionCommand(string PriceId, string SuccessUrl, string CancelUrl) : IRequest<StripeSessionDto>;

public class CreateCheckoutSessionCommandHandler(
    IStripeService stripeService,
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<CreateCheckoutSessionCommand, StripeSessionDto>
{
    public async Task<StripeSessionDto> Handle(CreateCheckoutSessionCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null) throw new NotFoundException(nameof(ApplicationUser), currentUserService.UserId!);

        if (string.IsNullOrWhiteSpace(user.StripeCustomerId))
        {
            user.StripeCustomerId = await stripeService.CreateCustomerAsync(user.Email!, user.FullName, ct);
            await userManager.UpdateAsync(user);
        }

        var url = await stripeService.CreateCheckoutSessionAsync(
            user.StripeCustomerId,
            request.PriceId,
            request.SuccessUrl,
            request.CancelUrl,
            ct);

        return new StripeSessionDto(url);
    }
}
