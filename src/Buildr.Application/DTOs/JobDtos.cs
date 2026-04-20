using Buildr.Domain.Enums;

namespace Buildr.Application.DTOs;

public record JobDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string Title,
    string? Description,
    JobStatus Status,
    DateTime? StartDate,
    DateTime? EndDate,
    decimal TotalCost,
    decimal TotalRevenue,
    decimal MarginPercent,
    IReadOnlyList<LineItemDto> LineItems,
    QuoteDto? Quote,
    InvoiceDto? Invoice,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record JobSummaryDto(
    Guid Id,
    Guid ClientId,
    string ClientName,
    string Title,
    JobStatus Status,
    DateTime? StartDate,
    DateTime? EndDate,
    decimal TotalCost,
    decimal TotalRevenue,
    decimal MarginPercent,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateJobRequest(
    Guid ClientId,
    string Title,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate);

public record UpdateJobRequest(
    Guid ClientId,
    string Title,
    string? Description,
    DateTime? StartDate,
    DateTime? EndDate);

public record UpdateStatusRequest(JobStatus Status);
