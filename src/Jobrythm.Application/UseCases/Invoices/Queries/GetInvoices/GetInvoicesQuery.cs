using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Queries.GetInvoices;

public record GetInvoicesQuery(string? Search, int Page, int PageSize) : IRequest<PagedResult<InvoiceDto>>;

public class GetInvoicesQueryHandler(
    IInvoiceRepository invoiceRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetInvoicesQuery, PagedResult<InvoiceDto>>
{
    public async Task<PagedResult<InvoiceDto>> Handle(GetInvoicesQuery request, CancellationToken ct)
    {
        var result = await invoiceRepository.GetPagedAsync(
            currentUserService.UserId!,
            status: null,
            request.Page,
            request.PageSize,
            ct);

        var dtos = mapper.Map<IReadOnlyList<InvoiceDto>>(result.Items);
        return new PagedResult<InvoiceDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}
