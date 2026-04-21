namespace Jobrythm.Application.DTOs;

public record ClientDto(
    Guid Id,
    string Name,
    string? Email,
    string? Phone,
    string? Address,
    int JobCount,
    long TotalRevenue,
    DateTime CreatedAt,
    DateTime UpdatedAt);

public record CreateClientRequest(
    string Name,
    string? Email,
    string? Phone,
    string? Address);

public record UpdateClientRequest(
    string Name,
    string? Email,
    string? Phone,
    string? Address);
