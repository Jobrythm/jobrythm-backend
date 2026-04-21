using Jobrythm.Application.DTOs;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.CreateInvoice;

public record CreateInvoiceCommand : IRequest<InvoiceDto>
{
    public Guid JobId { get; init; }
    public DateTime? DueDate { get; init; }
    public string? Notes { get; init; }
    public string? Terms { get; init; }
    public long TotalNet { get; init; }
    public int VatRate { get; init; }
    public long VatAmount { get; init; }
    public long TotalGross { get; init; }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    public Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}