using MediatR;

namespace Jobrythm.Application.UseCases.Users.Commands.UploadLogo;

public record UploadLogoCommand(Stream Stream, string FileName, string ContentType) : IRequest<string>;

public class UploadLogoCommandHandler : IRequestHandler<UploadLogoCommand, string>
{
    public Task<string> Handle(UploadLogoCommand request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}