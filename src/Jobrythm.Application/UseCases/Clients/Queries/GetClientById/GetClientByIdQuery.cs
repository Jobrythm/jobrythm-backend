using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Clients.Queries.GetClientById;

public record GetClientByIdQuery(Guid Id) : IRequest<ClientDto>;

public class GetClientByIdQueryHandler(
    IClientRepository clientRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<GetClientByIdQuery, ClientDto>
{
    public async Task<ClientDto> Handle(GetClientByIdQuery request, CancellationToken ct)
    {
        var client = await clientRepository.GetByIdAsync(request.Id, ct);
        
        if (client == null) throw new NotFoundException(nameof(client), request.Id);
        if (client.UserId != currentUserService.UserId) throw new ForbiddenException();

        var dto = mapper.Map<ClientDto>(client);
        dto = dto with 
        { 
            JobCount = await clientRepository.GetJobCountAsync(client.Id, ct),
            TotalRevenue = await clientRepository.GetTotalRevenueAsync(client.Id, ct)
        };

        return dto;
    }
}
