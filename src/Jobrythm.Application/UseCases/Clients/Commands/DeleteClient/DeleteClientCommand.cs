using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using MediatR;

namespace Jobrythm.Application.UseCases.Clients.Commands.DeleteClient;

public record DeleteClientCommand(Guid Id) : IRequest;

public class DeleteClientCommandHandler(
    IClientRepository clientRepository,
    ICurrentUserService currentUserService) : IRequestHandler<DeleteClientCommand>
{
    public async Task Handle(DeleteClientCommand request, CancellationToken ct)
    {
        var client = await clientRepository.GetByIdAsync(request.Id, ct);
        if (client == null) throw new NotFoundException(nameof(client), request.Id);

        if (client.UserId != currentUserService.UserId)
            throw new ForbiddenException();

        var jobCount = await clientRepository.GetJobCountAsync(request.Id, ct);
        if (jobCount > 0)
            throw new ConflictException("Cannot delete client with existing jobs.");

        clientRepository.Remove(client);
        await clientRepository.SaveChangesAsync(ct);
    }
}
