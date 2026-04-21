using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Jobrythm.Application.UseCases.Users.Commands.UpdateProfile;

public record UpdateProfileCommand : IRequest
{
    public string FullName { get; init; } = string.Empty;
    public string? CompanyName { get; init; }
    public string? CompanyAddress { get; init; }
    public decimal DefaultVatRate { get; init; }
    public string? DefaultPaymentTerms { get; init; }
    public int DefaultQuoteValidityDays { get; init; } = 30;
}

public class UpdateProfileCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICurrentUserService currentUserService) : IRequestHandler<UpdateProfileCommand>
{
    public async Task Handle(UpdateProfileCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(currentUserService.UserId!);
        if (user == null) throw new NotFoundException(nameof(ApplicationUser), currentUserService.UserId!);

        user.FullName = request.FullName;
        user.CompanyName = request.CompanyName;
        user.CompanyAddress = request.CompanyAddress;
        user.DefaultVatRate = request.DefaultVatRate;
        user.DefaultPaymentTerms = request.DefaultPaymentTerms;
        user.DefaultQuoteValidityDays = request.DefaultQuoteValidityDays;

        await userManager.UpdateAsync(user);
    }
}
