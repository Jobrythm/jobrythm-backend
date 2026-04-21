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

        var allJobs = await jobRepository.GetPagedAsync(userId, status: null, search: null, page: 1, pageSize: int.MaxValue, ct);
        var activeJobs = await jobRepository.GetPagedAsync(userId, JobStatus.Active, search: null, page: 1, pageSize: int.MaxValue, ct);
        var allClients = await clientRepository.GetPagedAsync(userId, search: null, page: 1, pageSize: int.MaxValue, ct);
        var pendingQuotes = await quoteRepository.GetPagedAsync(userId, QuoteStatus.Sent, page: 1, pageSize: int.MaxValue, ct);
        var paidInvoices = await invoiceRepository.GetPagedAsync(userId, InvoiceStatus.Paid, page: 1, pageSize: int.MaxValue, ct);
        var overdueInvoices = await invoiceRepository.GetPagedAsync(userId, InvoiceStatus.Overdue, page: 1, pageSize: int.MaxValue, ct);

        var totalRevenue = paidInvoices.Items.Sum(i => i.TotalGross) / 100m;
        var totalCost = allJobs.Items.Sum(j => j.TotalCost) / 100m;

        return new DashboardStatsDto(
            TotalJobs: allJobs.TotalCount,
            ActiveJobs: activeJobs.TotalCount,
            TotalClients: allClients.TotalCount,
            TotalRevenue: totalRevenue,
            TotalCost: totalCost,
            TotalMargin: totalRevenue - totalCost,
            PendingQuotes: pendingQuotes.TotalCount,
            OverdueInvoices: overdueInvoices.TotalCount
        );
    }
}
