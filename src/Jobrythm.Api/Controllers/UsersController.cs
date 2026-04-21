using Jobrythm.Application.UseCases.Users.Commands.UpdateProfile;
using Jobrythm.Application.UseCases.Users.Commands.UploadLogo;
using Jobrythm.Application.UseCases.Users.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Jobrythm.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<CurrentUserDto>> GetMe(CancellationToken ct)
    {
        return await mediator.Send(new GetCurrentUserQuery(), ct);
    }

    [HttpPut("me")]
    public async Task<IActionResult> UpdateMe(UpdateProfileCommand command, CancellationToken ct)
    {
        await mediator.Send(command, ct);
        return NoContent();
    }

    [HttpPost("me/logo")]
    public async Task<ActionResult<string>> UploadLogo(IFormFile file, CancellationToken ct)
    {
        if (file == null || file.Length == 0)
            return BadRequest("No file uploaded.");

        if (file.Length > 5 * 1024 * 1024)
            return BadRequest("File size exceeds 5MB.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return BadRequest("Invalid file type.");

        using var stream = file.OpenReadStream();
        var command = new UploadLogoCommand(stream, file.FileName, file.ContentType);
        var url = await mediator.Send(command, ct);
        
        return Ok(url);
    }
}
