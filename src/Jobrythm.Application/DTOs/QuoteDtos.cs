using Jobrythm.Domain.Enums;

namespace Jobrythm.Application.DTOs;

public record QuoteDto(
    Guid Id,
    Guid JobId,
    string QuoteNumber,
    QuoteStatus Status,
    DateTime? ValidUntil,
    string? Notes,
    string? Terms,
    long TotalNet,
    int VatRate,
    long VatAmount,
    long TotalGross,
    DateTime? SentAt,
    DateTime? AcceptedAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateQuoteRequest(
    DateTime? ValidUntil,
    string? Notes,
    string? Terms,
    long TotalNet,
    int VatRate,
    long VatAmount,
    long TotalGross);

public record UpdateQuoteRequest(
    DateTime? ValidUntil,
    string? Notes,
    string? Terms,
    long TotalNet,
    int VatRate,
    long VatAmount,
    long TotalGross);
