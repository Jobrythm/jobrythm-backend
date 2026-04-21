using System.Net.Http.Json;
using Jobrythm.Application.Interfaces;
using Jobrythm.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Jobrythm.Infrastructure.Services;

public class ResendEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly ResendOptions _options;
    private readonly IPdfService _pdfService;

    public ResendEmailService(HttpClient httpClient, IOptions<ResendOptions> options, IPdfService pdfService)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _pdfService = pdfService;
        _httpClient.BaseAddress = new Uri("https://api.resend.com/");
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _options.ApiKey);
    }

    public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken cancellationToken = default)
    {
        var request = new
        {
            from = _options.FromEmail,
            to,
            subject,
            html = htmlBody
        };

        await _httpClient.PostAsJsonAsync("emails", request, cancellationToken);
    }

    public async Task SendQuoteAsync(Guid quoteId, string recipientEmail, CancellationToken cancellationToken = default)
    {
        var pdfBytes = await _pdfService.GenerateQuotePdfAsync(quoteId, cancellationToken);
        var base64Pdf = Convert.ToBase64String(pdfBytes);

        var request = new
        {
            from = _options.FromEmail,
            to = recipientEmail,
            subject = "Your Quote",
            html = "<p>Please find your quote attached.</p>",
            attachments = new[]
            {
                new { filename = "quote.pdf", content = base64Pdf }
            }
        };

        await _httpClient.PostAsJsonAsync("emails", request, cancellationToken);
    }

    public async Task SendInvoiceAsync(Guid invoiceId, string recipientEmail, CancellationToken cancellationToken = default)
    {
        var pdfBytes = await _pdfService.GenerateInvoicePdfAsync(invoiceId, cancellationToken);
        var base64Pdf = Convert.ToBase64String(pdfBytes);

        var request = new
        {
            from = _options.FromEmail,
            to = recipientEmail,
            subject = "Your Invoice",
            html = "<p>Please find your invoice attached.</p>",
            attachments = new[]
            {
                new { filename = "invoice.pdf", content = base64Pdf }
            }
        };

        await _httpClient.PostAsJsonAsync("emails", request, cancellationToken);
    }
}
