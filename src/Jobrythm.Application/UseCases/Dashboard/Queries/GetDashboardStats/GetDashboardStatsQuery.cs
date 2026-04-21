using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Enums;
using MediatR;

namespace Jobrythm.Application.UseCases.Dashboard.Queries.GetDashboardStats;

public record GetDashboardStatsQuery : IRequest<DashboardStatsDto>;

public class GetDashboardStatsQueryHandler(
    IJobRepository jobRepository,
    IClientRepository clientRepository,
    IInvoiceRepository invoiceRepository,
    IQuoteRepository quoteRepository,
    ICurrentUserService currentUserService) : IRequestHandler<GetDashboardStatsQuery, DashboardStatsDto>
{
    public async Task<DashboardStatsDto> Handle(GetDashboardStatsQuery request, CancellationToken ct)
    {
        var userId = currentUserService.UserId!;

        return new DashboardStatsDto(
            TotalJobs: 0,
            ActiveJobs: 0,
            TotalClients: 0,
            TotalRevenue: 0m,
            TotalCost: 0m,
            TotalMargin: 0m,
            PendingQuotes: 0,
            OverdueInvoices: 0
        );
    }
}
