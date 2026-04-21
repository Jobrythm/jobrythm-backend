using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Quotes.Queries.GetQuoteById;

public record GetQuoteByIdQuery(Guid Id) : IRequest<QuoteDto>;

public class GetQuoteByIdQueryHandler(
    IQuoteRepository quoteRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetQuoteByIdQuery, QuoteDto>
{
    public async Task<QuoteDto> Handle(GetQuoteByIdQuery request, CancellationToken ct)
    {
        var quote = await quoteRepository.GetByIdAsync(request.Id, ct);
        
        if (quote == null) throw new NotFoundException(nameof(quote), request.Id);
        if (quote.Job.UserId != currentUserService.UserId) throw new ForbiddenException();

        return mapper.Map<QuoteDto>(quote);
    }
}
