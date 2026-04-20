using Buildr.Domain.Entities;

namespace Buildr.Application.Interfaces;

public interface IJwtTokenService
{
    string GenerateAccessToken(ApplicationUser user);
    string GenerateRefreshToken();

    /// <summary>Returns the user ID extracted from an expired access token (for refresh flows).</summary>
    string? GetUserIdFromExpiredToken(string accessToken);
}
