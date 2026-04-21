using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Jobrythm.Application.UseCases.Users.Commands.UploadLogo;

public record UploadLogoCommand(Stream Stream, string FileName, string ContentType) : IRequest<string>;

public class UploadLogoCommandHandler(
    IFileStorageService fileStorageService,
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<UploadLogoCommand, string>
{
    public async Task<string> Handle(UploadLogoCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null) throw new NotFoundException(nameof(ApplicationUser), currentUserService.UserId!);

        // Delete old logo if one exists
        if (!string.IsNullOrWhiteSpace(user.LogoUrl))
            await fileStorageService.DeleteAsync(user.LogoUrl, ct);

        var url = await fileStorageService.UploadAsync(request.Stream, request.FileName, request.ContentType, ct);

        user.LogoUrl = url;
        await userManager.UpdateAsync(user);

        return url;
    }
}
