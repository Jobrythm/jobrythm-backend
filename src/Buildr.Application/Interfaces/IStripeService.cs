namespace Buildr.Application.Interfaces;

public interface IStripeService
{
    Task<string> CreateCustomerAsync(string email, string name, CancellationToken cancellationToken = default);
    Task<string> CreateCheckoutSessionAsync(string customerId, string priceId, string successUrl, string cancelUrl, CancellationToken cancellationToken = default);
    Task<string> CreateBillingPortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken = default);
    Task HandleWebhookAsync(string payload, string signature, CancellationToken cancellationToken = default);
}
