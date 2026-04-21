using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Enums;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.SendInvoice;

public record SendInvoiceCommand(Guid Id) : IRequest;

public class SendInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    IEmailService emailService,
    ICurrentUserService currentUserService) : IRequestHandler<SendInvoiceCommand>
{
    public async Task Handle(SendInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.Id, ct);
        if (invoice == null) throw new NotFoundException(nameof(invoice), request.Id);
        if (invoice.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        await emailService.SendInvoiceAsync(invoice.Id, invoice.Job.Client.Email!, ct);

        invoice.Status = InvoiceStatus.Sent;
        invoice.SentAt = DateTime.UtcNow;
        await invoiceRepository.SaveChangesAsync(ct);
    }
}
