using Jobrythm.Application.DTOs;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Queries.GetInvoices;

public record GetInvoicesQuery(string? Search, int Page, int PageSize) : IRequest<PagedResult<InvoiceDto>>;

public class GetInvoicesQueryHandler : IRequestHandler<GetInvoicesQuery, PagedResult<InvoiceDto>>
{
    public Task<PagedResult<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}