using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Queries.GetInvoiceById;

public record GetInvoiceByIdQuery(Guid Id) : IRequest<InvoiceDto>;

public class GetInvoiceByIdQueryHandler(
    IInvoiceRepository invoiceRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetInvoiceByIdQuery, InvoiceDto>
{
    public async Task<InvoiceDto> Handle(GetInvoiceByIdQuery request, CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.Id, ct);
        if (invoice == null) throw new NotFoundException(nameof(invoice), request.Id);
        if (invoice.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        return mapper.Map<InvoiceDto>(invoice);
    }
}
