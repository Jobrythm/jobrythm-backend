using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;

namespace Jobrythm.Application.UseCases.Quotes.Commands.CreateQuote;

public class CreateQuoteCommand : IRequest<QuoteDto>
{
    public Guid JobId { get; set; }
    public int VatRate { get; set; } = 20;
    public DateTime? ValidUntil { get; set; }
    public string? Notes { get; set; }
}

public class CreateQuoteCommandHandler(
    IJobRepository jobRepository,
    IQuoteRepository quoteRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateQuoteCommand, QuoteDto>
{
    public async Task<QuoteDto> Handle(CreateQuoteCommand request, CancellationToken ct)
    {
        var job = await jobRepository.GetByIdWithDetailsAsync(request.JobId, ct);
        if (job == null) throw new NotFoundException(nameof(job), request.JobId);

        if (job.UserId != currentUserService.UserId)
            throw new ForbiddenException();

        var existingQuote = await quoteRepository.GetByJobIdAsync(request.JobId, ct);
        if (existingQuote != null)
            throw new ConflictException("A quote already exists for this job.");

        // Minimal logic for test
        var quoteNumber = $"QT-{Guid.NewGuid().ToString().Substring(0, 8)}";

        var netTotal = job.TotalRevenue;
        var vatAmount = (long)Math.Round(netTotal * (request.VatRate / 100.0));
        var grossTotal = netTotal + vatAmount;

        var quote = new Quote
        {
            JobId = request.JobId,
            QuoteNumber = quoteNumber,
            Status = Domain.Enums.QuoteStatus.Draft,
            VatRate = request.VatRate,
            TotalNet = netTotal,
            VatAmount = vatAmount,
            TotalGross = grossTotal,
            ValidUntil = request.ValidUntil ?? DateTime.UtcNow.AddDays(30),
            Notes = request.Notes
        };

        await quoteRepository.AddAsync(quote, ct);
        return mapper.Map<QuoteDto>(quote);
    }
}
