using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using Jobrythm.Application.UseCases.Auth.Commands.Login;
using Jobrythm.Application.UseCases.Auth.Commands.Logout;
using Jobrythm.Application.UseCases.Auth.Commands.Register;
using Jobrythm.Application.UseCases.Auth.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/auth")]
[EnableRateLimiting("auth")]
public class AuthController(IMediator mediator, ICurrentUserService currentUserService) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterCommand command, CancellationToken ct)
    {
        var result = await mediator.Send(command, ct);
        return StatusCode(201, result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Refresh(RefreshTokenCommand command, CancellationToken ct)
    {
        return await mediator.Send(command, ct);
    }

    [HttpPost("logout")]
    [Authorize]
    public async Task<IActionResult> Logout(LogoutRequest request, CancellationToken ct)
    {
        var command = new LogoutCommand 
        { 
            UserId = currentUserService.UserId!, 
            RefreshToken = request.RefreshToken 
        };
        await mediator.Send(command, ct);
        return NoContent();
    }
}

public class LogoutRequest
{
    public string RefreshToken { get; set; } = string.Empty;
}
