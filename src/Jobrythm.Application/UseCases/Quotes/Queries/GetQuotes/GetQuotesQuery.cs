using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Quotes.Queries.GetQuotes;

public record GetQuotesQuery(string? Search, int Page, int PageSize, Domain.Enums.QuoteStatus? Status = null) : IRequest<PagedResult<QuoteDto>>;

public class GetQuotesQueryHandler(
    IQuoteRepository quoteRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetQuotesQuery, PagedResult<QuoteDto>>
{
    public async Task<PagedResult<QuoteDto>> Handle(GetQuotesQuery request, CancellationToken ct)
    {
        var result = await quoteRepository.GetPagedAsync(
            currentUserService.UserId!, 
            request.Status, 
            request.Page, 
            request.PageSize, 
            ct);

        var dtos = mapper.Map<IReadOnlyList<QuoteDto>>(result.Items);
        return new PagedResult<QuoteDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}
