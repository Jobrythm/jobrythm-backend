namespace Jobrythm.Application.Interfaces;

public interface INumberSequenceService
{
    Task<string> NextAsync(string userId, string prefix, CancellationToken cancellationToken = default);
}