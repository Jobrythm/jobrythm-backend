using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Application.UseCases.Auth.Commands.Logout;

public record LogoutCommand : IRequest
{
    public string UserId { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
}

public class LogoutCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService) : IRequestHandler<LogoutCommand>
{
    public async Task Handle(LogoutCommand request, CancellationToken ct)
    {
        var user = await userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user == null) return;

        var tokenHash = jwtTokenService.HashToken(request.RefreshToken);
        var token = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash);
        if (token != null)
        {
            user.RefreshTokens.Remove(token);
            await userManager.UpdateAsync(user);
        }
    }
}
