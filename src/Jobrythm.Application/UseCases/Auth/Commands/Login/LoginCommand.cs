using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Jobrythm.Application.UseCases.Auth.Commands.Login;

public record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;

public class LoginCommandHandler(
    UserManager<ApplicationUser> userManager,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
            throw new BadRequestException("Invalid email or password.");

        var accessToken = jwtTokenService.GenerateAccessToken(user);
        var refreshToken = jwtTokenService.GenerateRefreshToken();

        user.RefreshTokens.Add(new Domain.Entities.RefreshToken
        {
            TokenHash = "HASHED_TOKEN", // Hashing should happen in service
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        });
        await userManager.UpdateAsync(user);

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            user.Id,
            user.Email!,
            user.FullName
        );
    }
}
