using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;

namespace Jobrythm.Application.UseCases.Clients.Commands.CreateClient;

public class CreateClientCommand : IRequest<ClientDto>
{
    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }
}

public class CreateClientCommandHandler(
    IClientRepository clientRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateClientCommand, ClientDto>
{
    public async Task<ClientDto> Handle(CreateClientCommand request, CancellationToken ct)
    {
        var client = new Client
        {
            UserId = currentUserService.UserId!,
            Name = request.Name,
            Email = request.Email,
            Phone = request.Phone,
            Address = request.Address
        };

        await clientRepository.AddAsync(client, ct);
        await clientRepository.SaveChangesAsync(ct);

        return mapper.Map<ClientDto>(client);
    }
}
