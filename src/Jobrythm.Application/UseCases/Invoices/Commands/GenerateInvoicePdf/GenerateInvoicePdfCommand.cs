using Jobrythm.Application.DTOs;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.GenerateInvoicePdf;

public record GenerateInvoicePdfCommand(Guid Id) : IRequest<FileResponse>;

public class GenerateInvoicePdfCommandHandler : IRequestHandler<GenerateInvoicePdfCommand, FileResponse>
{
    public Task<FileResponse> Handle(GenerateInvoicePdfCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}