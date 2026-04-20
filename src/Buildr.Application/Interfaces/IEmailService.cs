namespace Buildr.Application.Interfaces;

public interface IEmailService
{
    Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default);
    Task SendQuoteAsync(Guid quoteId, string recipientEmail, CancellationToken cancellationToken = default);
    Task SendInvoiceAsync(Guid invoiceId, string recipientEmail, CancellationToken cancellationToken = default);
}
