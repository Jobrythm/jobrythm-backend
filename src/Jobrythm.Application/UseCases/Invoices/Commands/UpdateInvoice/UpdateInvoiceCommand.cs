using Jobrythm.Application.DTOs;
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

public class UpdateInvoiceCommandHandler : IRequestHandler<UpdateInvoiceCommand>
{
    public Task Handle(UpdateInvoiceCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}