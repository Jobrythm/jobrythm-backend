namespace Jobrythm.Application.DTOs;

public record DashboardStatsDto(
    int TotalJobs,
    int ActiveJobs,
    int TotalClients,
    decimal TotalRevenue,
    decimal TotalCost,
    decimal TotalMargin,
    int PendingQuotes,
    int OverdueInvoices);

public record ActivityItemDto(
    string Type,
    string Description,
    Guid EntityId,
    DateTime OccurredAt);
