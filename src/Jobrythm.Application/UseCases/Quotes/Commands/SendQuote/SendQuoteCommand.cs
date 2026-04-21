using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Quotes.Commands.SendQuote;

public record SendQuoteCommand(Guid Id) : IRequest;

public class SendQuoteCommandHandler(
    IQuoteRepository quoteRepository,
    IEmailService emailService,
    ICurrentUserService currentUserService) : IRequestHandler<SendQuoteCommand>
{
    public async Task Handle(SendQuoteCommand request, CancellationToken ct)
    {
        var quote = await quoteRepository.GetByIdAsync(request.Id, ct);
        if (quote == null) throw new NotFoundException(nameof(quote), request.Id);
        if (quote.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        await emailService.SendQuoteAsync(quote.Id, quote.Job.Client.Email!, ct);

        quote.Status = Domain.Enums.QuoteStatus.Sent;
        quote.SentAt = DateTime.UtcNow;
        await quoteRepository.SaveChangesAsync(ct);
    }
}
