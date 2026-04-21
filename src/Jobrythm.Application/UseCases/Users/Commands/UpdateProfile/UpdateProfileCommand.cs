using MediatR;

namespace Jobrythm.Application.UseCases.Users.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? CompanyName { get; init; }
    public string? CompanyAddress { get; init; }
    public decimal DefaultVatRate { get; init; }
}

public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand>
{
    public Task Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}