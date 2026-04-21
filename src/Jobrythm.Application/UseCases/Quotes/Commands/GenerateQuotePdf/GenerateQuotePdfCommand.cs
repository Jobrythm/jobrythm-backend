using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Quotes.Commands.GenerateQuotePdf;

public record GenerateQuotePdfCommand(Guid Id) : IRequest<PdfResult>;

public record PdfResult(byte[] Content, string FileName);

public class GenerateQuotePdfCommandHandler(
    IQuoteRepository quoteRepository,
    IPdfService pdfService,
    ICurrentUserService currentUserService) : IRequestHandler<GenerateQuotePdfCommand, PdfResult>
{
    public async Task<PdfResult> Handle(GenerateQuotePdfCommand request, CancellationToken ct)
    {
        var quote = await quoteRepository.GetByIdAsync(request.Id, ct);
        if (quote == null) throw new NotFoundException(nameof(quote), request.Id);
        if (quote.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        var content = await pdfService.GenerateQuotePdfAsync(quote.Id, ct);
        return new PdfResult(content, $"Quote_{quote.QuoteNumber}.pdf");
    }
}
