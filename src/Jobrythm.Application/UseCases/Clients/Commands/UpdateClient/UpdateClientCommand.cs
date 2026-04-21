using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Clients.Commands.UpdateClient;

public class UpdateClientCommand : IRequest<ClientDto>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class UpdateClientCommandHandler(
    IClientRepository clientRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<UpdateClientCommand, ClientDto>
{
    public async Task<ClientDto> Handle(UpdateClientCommand request, CancellationToken ct)
    {
        var client = await clientRepository.GetByIdAsync(request.Id, ct);
        if (client == null) throw new NotFoundException(nameof(client), request.Id);
        if (client.UserId != currentUserService.UserId) throw new ForbiddenException();

        client.Name = request.Name;
        client.Email = request.Email;
        client.Phone = request.Phone;
        client.Address = request.Address;

        await clientRepository.SaveChangesAsync(ct);

        var dto = mapper.Map<ClientDto>(client);
        return dto with 
        { 
            JobCount = await clientRepository.GetJobCountAsync(client.Id, ct),
            TotalRevenue = await clientRepository.GetTotalRevenueAsync(client.Id, ct)
        };
    }
}
