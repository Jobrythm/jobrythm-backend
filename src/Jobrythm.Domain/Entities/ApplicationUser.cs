using Jobrythm.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace Jobrythm.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public string? CompanyName { get; set; }
    public string? CompanyAddress { get; set; }
    public string? LogoUrl { get; set; }

    public decimal DefaultVatRate { get; set; } = 20;
    public string? DefaultPaymentTerms { get; set; }
    public int DefaultQuoteValidityDays { get; set; } = 30;

    public SubscriptionPlan Plan { get; set; } = SubscriptionPlan.Starter;

    public string? StripeCustomerId { get; set; }
    public string? StripeSubscriptionId { get; set; }
    public DateTime? SubscriptionEndsAt { get; set; }

    public ICollection<Client> Clients { get; set; } = [];
    public ICollection<Job> Jobs { get; set; } = [];
    public ICollection<RefreshToken> RefreshTokens { get; set; } = [];
}
