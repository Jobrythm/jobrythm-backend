namespace Jobrythm.Application.DTOs;

public record FileResponse(byte[] Content, string FileName, string ContentType);