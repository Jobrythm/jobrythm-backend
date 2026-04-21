using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Jobrythm.Application.UseCases.Billing.Commands.CreatePortalSession;

public record CreatePortalSessionCommand(string ReturnUrl) : IRequest<StripeSessionDto>;

public class CreatePortalSessionCommandHandler(
    IStripeService stripeService,
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<CreatePortalSessionCommand, StripeSessionDto>
{
    public async Task<StripeSessionDto> Handle(CreatePortalSessionCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null) throw new NotFoundException(nameof(ApplicationUser), currentUserService.UserId!);

        if (string.IsNullOrWhiteSpace(user.StripeCustomerId))
            throw new BadRequestException("No billing account found. Please subscribe first.");

        var url = await stripeService.CreatePortalSessionAsync(user.StripeCustomerId, request.ReturnUrl, ct);
        return new StripeSessionDto(url);
    }
}
