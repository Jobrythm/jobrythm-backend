using Jobrythm.Application.DTOs;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Queries.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid Id) : IRequest<InvoiceDto>;

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    public Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}