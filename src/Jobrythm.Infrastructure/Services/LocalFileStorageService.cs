using Jobrythm.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Jobrythm.Infrastructure.Services;

public class LocalFileStorageService(IHttpContextAccessor httpContextAccessor) : IFileStorageService
{
    private readonly string _uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

    public async Task<string> UploadAsync(Stream stream, string fileName, string contentType, CancellationToken cancellationToken)
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value 
                     ?? "anonymous";
        
        var userPath = Path.Combine(_uploadPath, userId);
        if (!Directory.Exists(userPath))
        {
            Directory.CreateDirectory(userPath);
        }

        var filePath = Path.Combine(userPath, fileName);
        using var fileStream = new FileStream(filePath, FileMode.Create);
        await stream.CopyToAsync(fileStream, cancellationToken);

        return $"/uploads/{userId}/{fileName}";
    }

    public Task DeleteAsync(string fileUrl, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(fileUrl)) return Task.CompletedTask;

        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }

    public Task<Stream> DownloadAsync(string fileUrl, CancellationToken cancellationToken)
    {
        var relativePath = fileUrl.TrimStart('/');
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath);

        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException("File not found", fullPath);
        }

        return Task.FromResult<Stream>(new FileStream(fullPath, FileMode.Open, FileAccess.Read));
    }
}
