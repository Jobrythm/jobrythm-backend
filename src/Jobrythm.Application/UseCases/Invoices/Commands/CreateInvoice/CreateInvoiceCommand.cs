using AutoMapper;
using Jobrythm.Application.DTOs;
using Jobrythm.Application.Exceptions;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using MediatR;

namespace Jobrythm.Application.UseCases.Invoices.Commands.CreateInvoice;

public record CreateInvoiceCommand : IRequest<InvoiceDto>
{
    public Guid JobId { get; init; }
    public DateTime? DueDate { get; init; }
    public string? Notes { get; init; }
    public string? Terms { get; init; }
    public long TotalNet { get; init; }
    public int VatRate { get; init; }
    public long VatAmount { get; init; }
    public long TotalGross { get; init; }
}

public class CreateInvoiceCommandHandler(
    IJobRepository jobRepository,
    IInvoiceRepository invoiceRepository,
    ICurrentUserService currentUserService,
    IMapper mapper) : IRequestHandler<CreateInvoiceCommand, InvoiceDto>
{
    public async Task<InvoiceDto> Handle(CreateInvoiceCommand request, CancellationToken ct)
    {
        var job = await jobRepository.GetByIdWithDetailsAsync(request.JobId, ct);
        if (job == null) throw new NotFoundException(nameof(job), request.JobId);
        if (job.UserId != currentUserService.UserId) throw new ForbiddenException();

        var existing = await invoiceRepository.GetByJobIdAsync(request.JobId, ct);
        if (existing != null) throw new ConflictException("An invoice already exists for this job.");

        var invoiceNumber = $"INV-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        var invoice = new Invoice
        {
            JobId = request.JobId,
            InvoiceNumber = invoiceNumber,
            Status = Domain.Enums.InvoiceStatus.Draft,
            DueDate = request.DueDate,
            Notes = request.Notes,
            Terms = request.Terms,
            TotalNet = request.TotalNet,
            VatRate = request.VatRate,
            VatAmount = request.VatAmount,
            TotalGross = request.TotalGross
        };

        await invoiceRepository.AddAsync(invoice, ct);
        await invoiceRepository.SaveChangesAsync(ct);

        return mapper.Map<InvoiceDto>(invoice);
    }
}
