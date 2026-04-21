using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Infrastructure.Services;

public class NumberSequenceService : INumberSequenceService
{
    private readonly INumberSequenceRepository _repository;

    public NumberSequenceService(INumberSequenceRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> NextAsync(string userId, string prefix, CancellationToken cancellationToken = default)
    {
        var sequence = await _repository.GetAsync(userId, prefix, cancellationToken);

        if (sequence == null)
        {
            sequence = new NumberSequence
            {
                UserId = userId,
                Prefix = prefix,
                LastNumber = 0
            };
            await _repository.AddAsync(sequence, cancellationToken);
        }

        sequence.LastNumber++;
        await _repository.SaveChangesAsync(cancellationToken);

        return $"{prefix}-{sequence.LastNumber:D4}";
    }
}
