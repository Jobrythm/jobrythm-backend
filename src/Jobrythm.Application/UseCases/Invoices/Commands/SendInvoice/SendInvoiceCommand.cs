using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.SendInvoice;

public record SendInvoiceCommand(Guid Id) : IRequest;

public class SendInvoiceCommandHandler : IRequestHandler<SendInvoiceCommand>
{
    public Task Handle(SendInvoiceCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}