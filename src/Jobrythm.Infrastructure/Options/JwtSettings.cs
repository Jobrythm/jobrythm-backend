namespace Jobrythm.Infrastructure.Options;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string[] ValidIssuers { get; set; } = [];
    public string[] ValidAudiences { get; set; } = [];
    public int ExpiryMinutes { get; set; } = 60;
}
