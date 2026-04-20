using Buildr.Domain.Enums;

namespace Buildr.Application.DTOs;

public record InvoiceDto(
    Guid Id,
    Guid JobId,
    string InvoiceNumber,
    InvoiceStatus Status,
    DateTime? DueDate,
    string? Notes,
    string? Terms,
    long TotalNet,
    int VatRate,
    long VatAmount,
    long TotalGross,
    DateTime? SentAt,
    DateTime? PaidAt,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateInvoiceRequest(
    DateTime? DueDate,
    string? Notes,
    string? Terms,
    long TotalNet,
    int VatRate,
    long VatAmount,
    long TotalGross);

public record UpdateInvoiceRequest(
    DateTime? DueDate,
    string? Notes,
    string? Terms,
    long TotalNet,
    int VatRate,
    long VatAmount,
    long TotalGross);
