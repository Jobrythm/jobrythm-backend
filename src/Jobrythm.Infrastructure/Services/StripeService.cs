using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Jobrythm.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Stripe.BillingPortal;

namespace Jobrythm.Infrastructure.Services;

public class StripeService : IStripeService
{
    private readonly StripeOptions _options;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ILogger<StripeService> _logger;

    public StripeService(
        IOptions<StripeOptions> options,
        UserManager<ApplicationUser> userManager,
        ILogger<StripeService> logger)
    {
        _options = options.Value;
        _userManager = userManager;
        _logger = logger;
        StripeConfiguration.ApiKey = _options.ApiKey;
    }

    public async Task<string> CreateCustomerAsync(string email, string name, CancellationToken cancellationToken)
    {
        var options = new CustomerCreateOptions
        {
            Email = email,
            Name = name
        };
        var service = new CustomerService();
        var customer = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return customer.Id;
    }

    public async Task<string> CreateCheckoutSessionAsync(string customerId, string priceId, string successUrl, string cancelUrl, CancellationToken cancellationToken)
    {
        var options = new Stripe.Checkout.SessionCreateOptions
        {
            Customer = customerId,
            PaymentMethodTypes = new List<string> { "card" },
            LineItems = new List<SessionLineItemOptions> { new SessionLineItemOptions { Price = priceId, Quantity = 1 } },
            Mode = "subscription",
            SuccessUrl = successUrl,
            CancelUrl = cancelUrl
        };
        var service = new Stripe.Checkout.SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return session.Url;
    }

    public async Task<string> CreatePortalSessionAsync(string customerId, string returnUrl, CancellationToken cancellationToken)
    {
        var options = new Stripe.BillingPortal.SessionCreateOptions
        {
            Customer = customerId,
            ReturnUrl = returnUrl
        };
        var service = new Stripe.BillingPortal.SessionService();
        var session = await service.CreateAsync(options, cancellationToken: cancellationToken);
        return session.Url;
    }

    public async Task HandleWebhookAsync(string json, string stripeSignature, CancellationToken cancellationToken)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, _options.WebhookSecret);

            switch (stripeEvent.Type)
            {
                case EventTypes.CustomerSubscriptionUpdated:
                    await HandleSubscriptionUpdated(stripeEvent, cancellationToken);
                    break;
                case EventTypes.CustomerSubscriptionDeleted:
                    await HandleSubscriptionDeleted(stripeEvent, cancellationToken);
                    break;
                case EventTypes.InvoicePaymentSucceeded:
                    _logger.LogInformation("Stripe Invoice Payment Succeeded: {EventId}", stripeEvent.Id);
                    break;
            }
        }
        catch (StripeException e)
        {
            _logger.LogError(e, "Stripe Webhook Error");
            throw;
        }
    }

    private async Task HandleSubscriptionUpdated(Event stripeEvent, CancellationToken cancellationToken)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StripeCustomerId == subscription.CustomerId, cancellationToken);
        if (user == null) return;

        var priceId = subscription.Items.Data[0].Price.Id;
        user.Plan = priceId == _options.ProPriceId ? SubscriptionPlan.Pro :
                    priceId == _options.TeamPriceId ? SubscriptionPlan.Team :
                    SubscriptionPlan.Starter;

        user.StripeSubscriptionId = subscription.Id;
        user.SubscriptionEndsAt = subscription.CurrentPeriodEnd;
        await _userManager.UpdateAsync(user);
    }

    private async Task HandleSubscriptionDeleted(Event stripeEvent, CancellationToken cancellationToken)
    {
        var subscription = stripeEvent.Data.Object as Subscription;
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.StripeCustomerId == subscription.CustomerId, cancellationToken);
        if (user == null) return;

        user.Plan = SubscriptionPlan.Starter;
        user.StripeSubscriptionId = null;
        user.SubscriptionEndsAt = DateTime.UtcNow;
        await _userManager.UpdateAsync(user);
    }
}
