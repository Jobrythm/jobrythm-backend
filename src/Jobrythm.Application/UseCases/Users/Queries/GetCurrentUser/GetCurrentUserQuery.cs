using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Jobrythm.Application.UseCases.Users.Queries.GetCurrentUser;

public record CurrentUserDto(
    string Id,
    string Email,
    string FullName,
    string? CompanyName,
    string? CompanyAddress,
    string? LogoUrl,
    decimal DefaultVatRate,
    string? SubscriptionStatus);

public record GetCurrentUserQuery : IRequest<CurrentUserDto>;

public class GetCurrentUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public async Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null) throw new NotFoundException(nameof(ApplicationUser), currentUserService.UserId!);

        var subscriptionStatus = user.Plan switch
        {
            SubscriptionPlan.Pro => "Pro",
            SubscriptionPlan.Team => "Team",
            _ => "Starter"
        };

        return new CurrentUserDto(
            user.Id,
            user.Email!,
            user.FullName,
            user.CompanyName,
            user.CompanyAddress,
            user.LogoUrl,
            user.DefaultVatRate,
            subscriptionStatus
        );
    }
}
