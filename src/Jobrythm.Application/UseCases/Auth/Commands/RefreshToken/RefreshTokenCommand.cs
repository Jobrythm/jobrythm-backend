using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Application.UseCases.Auth.Commands.RefreshToken;

public record RefreshTokenCommand(string UserId, string RefreshToken) : IRequest<AuthResponse>;

public class RefreshTokenCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService) : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var user = await userManager.Users
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, ct);

        if (user == null)
            throw new UnauthorizedException();

        var tokenHash = jwtTokenService.HashToken(request.RefreshToken);
        var storedToken = user.RefreshTokens.FirstOrDefault(t => t.TokenHash == tokenHash);
        if (storedToken == null || storedToken.ExpiresAt < DateTime.UtcNow)
            throw new UnauthorizedException();

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var newRefreshToken = jwtTokenService.GenerateRefreshToken();

        storedToken.TokenHash = jwtTokenService.HashToken(newRefreshToken);
        storedToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
        await userManager.UpdateAsync(user);

        return new AuthResponse(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddHours(1),
            user.Id,
            user.Email!,
            user.FullName
        );
    }
}
