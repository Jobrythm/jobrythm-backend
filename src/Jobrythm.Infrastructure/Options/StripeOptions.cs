namespace Jobrythm.Infrastructure.Options;

public class StripeOptions
{
    public string ApiKey { get; set; } = string.Empty;
    public string WebhookSecret { get; set; } = string.Empty;
    public string ProPriceId { get; set; } = string.Empty;
    public string TeamPriceId { get; set; } = string.Empty;
}
