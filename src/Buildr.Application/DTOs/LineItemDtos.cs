using Buildr.Domain.Enums;

namespace Buildr.Application.DTOs;

public record LineItemDto(
    Guid Id,
    Guid JobId,
    string Description,
    LineItemCategory Category,
    decimal Quantity,
    string? Unit,
    long UnitCost,
    long UnitPrice,
    long TotalCost,
    long TotalPrice,
    decimal MarginPercent,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateLineItemRequest(
    string Description,
    LineItemCategory Category,
    decimal Quantity,
    string? Unit,
    long UnitCost,
    long UnitPrice);

public record UpdateLineItemRequest(
    string Description,
    LineItemCategory Category,
    decimal Quantity,
    string? Unit,
    long UnitCost,
    long UnitPrice);
