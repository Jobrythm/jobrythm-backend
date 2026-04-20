namespace Buildr.Application.Interfaces;

public interface IPdfService
{
    Task<byte[]> GenerateQuotePdfAsync(Guid quoteId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken = default);
}
