using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Quotes.Commands.UpdateQuote;

public class UpdateQuoteCommand : IRequest<QuoteDto>
{
    public Guid Id { get; set; }
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
    public string? Terms { get; set; }
    public long TotalNet { get; set; }
    public int VatRate { get; set; }
    public long VatAmount { get; set; }
    public long TotalGross { get; set; }
}

public class UpdateQuoteCommandHandler(
    IQuoteRepository quoteRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateQuoteCommand, QuoteDto>
{
    public async Task<QuoteDto> Handle(UpdateQuoteCommand request, CancellationToken ct)
    {
        var quote = await quoteRepository.GetByIdAsync(request.Id, ct);
        if (quote == null) throw new NotFoundException(nameof(quote), request.Id);
        if (quote.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        quote.ValidUntil = request.ValidUntil;
        quote.Notes = request.Notes;
        quote.Terms = request.Terms;
        quote.TotalNet = request.TotalNet;
        quote.VatRate = request.VatRate;
        quote.VatAmount = request.VatAmount;
        quote.TotalGross = request.TotalGross;

        await quoteRepository.SaveChangesAsync(ct);

        return mapper.Map<QuoteDto>(quote);
    }
}
