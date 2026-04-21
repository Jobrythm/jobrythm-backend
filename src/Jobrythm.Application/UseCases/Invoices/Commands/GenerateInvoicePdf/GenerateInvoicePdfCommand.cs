using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.GenerateInvoicePdf;

public record GenerateInvoicePdfCommand(Guid Id) : IRequest<FileResponse>;

public class GenerateInvoicePdfCommandHandler(
    IInvoiceRepository invoiceRepository,
    IPdfService pdfService,
    ICurrentUserService currentUserService) : IRequestHandler<GenerateInvoicePdfCommand, FileResponse>
{
    public async Task<FileResponse> Handle(GenerateInvoicePdfCommand request, CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.Id, ct);
        if (invoice == null) throw new NotFoundException(nameof(invoice), request.Id);
        if (invoice.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        var content = await pdfService.GenerateInvoicePdfAsync(invoice.Id, ct);
        return new FileResponse(content, $"Invoice_{invoice.InvoiceNumber}.pdf", "application/pdf");
    }
}
