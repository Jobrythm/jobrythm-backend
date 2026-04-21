using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Clients.Queries.GetClients;

public record GetClientsQuery(string? Search, int Page, int PageSize) : IRequest<PagedResult<ClientDto>>;

public class GetClientsQueryHandler(
    IClientRepository clientRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetClientsQuery, PagedResult<ClientDto>>
{
    public async Task<PagedResult<ClientDto>> Handle(GetClientsQuery request, CancellationToken ct)
    {
        var result = await clientRepository.GetPagedAsync(
            currentUserService.UserId!, 
            request.Search, 
            request.Page, 
            request.PageSize, 
            ct);

        var dtos = mapper.Map<IReadOnlyList<ClientDto>>(result.Items);
        return new PagedResult<ClientDto>(dtos, result.TotalCount, result.PageNumber, result.PageSize);
    }
}
