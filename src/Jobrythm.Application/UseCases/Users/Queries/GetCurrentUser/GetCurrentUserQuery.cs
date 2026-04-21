using Jobrythm.Application.DTOs;
using MediatR;

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

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserDto>
{
    public Task<CurrentUserDto> Handle(GetCurrentUserQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}