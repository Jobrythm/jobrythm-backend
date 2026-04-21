using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.UpdateInvoice;

public record UpdateInvoiceCommand : IRequest
{
    public Guid Id { get; init; }
    public DateTime? DueDate { get; init; }
    public string? Notes { get; init; }
    public string? Terms { get; init; }
    public long TotalNet { get; init; }
    public int VatRate { get; init; }
    public long VatAmount { get; init; }
    public long TotalGross { get; init; }
}

public class UpdateInvoiceCommandHandler(
    IInvoiceRepository invoiceRepository,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateInvoiceCommand>
{
    public async Task Handle(UpdateInvoiceCommand request, CancellationToken ct)
    {
        var invoice = await invoiceRepository.GetByIdAsync(request.Id, ct);
        if (invoice == null) throw new NotFoundException(nameof(invoice), request.Id);
        if (invoice.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        invoice.DueDate = request.DueDate;
        invoice.Notes = request.Notes;
        invoice.Terms = request.Terms;
        invoice.TotalNet = request.TotalNet;
        invoice.VatRate = request.VatRate;
        invoice.VatAmount = request.VatAmount;
        invoice.TotalGross = request.TotalGross;

        await invoiceRepository.SaveChangesAsync(ct);
    }
}
