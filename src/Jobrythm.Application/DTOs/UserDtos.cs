using Jobrythm.Domain.Enums;

namespace Jobrythm.Application.DTOs;

public record UserDto(
    string Id,
    string Email,
    string FullName,
    string? CompanyName,
    string? CompanyAddress,
    string? LogoUrl,
    decimal DefaultVatRate,
    string? DefaultPaymentTerms,
    int DefaultQuoteValidityDays,
    SubscriptionPlan Plan,
    DateTime? SubscriptionEndsAt);

public record UpdateProfileRequest(
    string FullName,
    string? CompanyName,
    string? CompanyAddress,
    string? LogoUrl,
    decimal DefaultVatRate,
    string? DefaultPaymentTerms,
    int DefaultQuoteValidityDays);
