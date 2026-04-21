using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Enums;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.MarkPaid;

public record MarkInvoicePaidCommand(Guid Id) : IRequest<InvoiceDto>;

public class MarkInvoicePaidCommandHandler(
    IInvoiceRepository invoiceRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<MarkInvoicePaidCommand, InvoiceDto>
{
    public async Task<InvoiceDto> Handle(MarkInvoicePaidCommand request, CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.Id, ct);
        if (invoice == null) throw new NotFoundException(nameof(invoice), request.Id);

        // Accessing Job through navigation property to check ownership
        if (invoice.Job.UserId != currentUserService.UserId)
            throw new ForbiddenException();

        invoice.Status = InvoiceStatus.Paid;
        invoice.PaidAt = DateTime.UtcNow;

        await invoiceRepository.SaveChangesAsync(ct);
        return mapper.Map<InvoiceDto>(invoice);
    }
}
