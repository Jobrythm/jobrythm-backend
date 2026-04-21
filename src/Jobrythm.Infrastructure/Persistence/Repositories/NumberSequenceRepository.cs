using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Jobrythm.Infrastructure.Persistence.Repositories;

public class NumberSequenceRepository : INumberSequenceRepository
{
    private readonly JobrythmDbContext _context;

    public NumberSequenceRepository(JobrythmDbContext context)
    {
        _context = context;
    }

    public async Task<NumberSequence?> GetAsync(string userId, string prefix, CancellationToken cancellationToken)
    {
        return await _context.NumberSequences
            .FirstOrDefaultAsync(s => s.UserId == userId && s.Prefix == prefix, cancellationToken);
    }

    public async Task AddAsync(NumberSequence sequence, CancellationToken cancellationToken)
    {
        await _context.NumberSequences.AddAsync(sequence, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
