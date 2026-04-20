namespace Buildr.Application.DTOs;

public record RegisterRequest(
    string Email,
    string Password,
    string FullName,
    string? CompanyName);

public record LoginRequest(
    string Email,
    string Password);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    string UserId,
    string Email,
    string FullName);

public record RefreshTokenRequest(
    string UserId,
    string RefreshToken);
