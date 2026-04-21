using System.Globalization;
using Jobrythm.Application.Interfaces;
using Jobrythm.Domain.Entities;
using Jobrythm.Domain.Enums;
using Jobrythm.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Jobrythm.Infrastructure.Services;

public class QuestPdfService : IPdfService
{
    private readonly JobrythmDbContext _context;

    public QuestPdfService(JobrythmDbContext context)
    {
        _context = context;
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public async Task<byte[]> GenerateQuotePdfAsync(Guid quoteId, CancellationToken cancellationToken)
    {
        var quote = await _context.Quotes
            .Include(q => q.Job)
            .ThenInclude(j => j.Client)
            .Include(q => q.Job)
            .ThenInclude(j => j.LineItems)
            .Include(q => q.Job)
            .ThenInclude(j => j.User)
            .FirstOrDefaultAsync(q => q.Id == quoteId, cancellationToken);

        if (quote == null) return [];

        return GenerateDocument("QUOTE", quote.QuoteNumber, quote.CreatedAt, quote.ValidUntil, quote.Job);
    }

    public async Task<byte[]> GenerateInvoicePdfAsync(Guid invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await _context.Invoices
            .Include(i => i.Job)
            .ThenInclude(j => j.Client)
            .Include(i => i.Job)
            .ThenInclude(j => j.LineItems)
            .Include(i => i.Job)
            .ThenInclude(j => j.User)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);

        if (invoice == null) return [];

        return GenerateDocument("INVOICE", invoice.InvoiceNumber, invoice.CreatedAt, invoice.DueDate, invoice.Job);
    }

    private byte[] GenerateDocument(string title, string number, DateTime date, DateTime? dueDate, Job job)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(50);
                page.Size(PageSizes.A4);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Helvetica"));

                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(job.User.CompanyName ?? "Jobrythm User").FontSize(20).SemiBold().FontColor(Colors.Blue.Medium);
                        col.Item().Text(job.User.CompanyAddress ?? "");
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                        col.Item().Text(title).FontSize(24).ExtraBold();
                        col.Item().Text($"#{number}");
                        col.Item().Text($"Date: {date:dd/MM/yyyy}");
                        if (dueDate.HasValue)
                            col.Item().Text($"Due Date: {dueDate.Value:dd/MM/yyyy}");
                    });
                });

                page.Content().PaddingVertical(20).Column(col =>
                {
                    col.Item().PaddingBottom(20).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Prepared For:").SemiBold();
                            c.Item().Text(job.Client.Name);
                            c.Item().Text(job.Client.Address ?? "");
                            c.Item().Text(job.Client.Email ?? "");
                        });
                    });

                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(3);
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Description");
                            header.Cell().Element(HeaderStyle).Text("Category");
                            header.Cell().Element(HeaderStyle).AlignCenter().Text("Qty");
                            header.Cell().Element(HeaderStyle).Text("Unit");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Unit Price");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Total");

                            static IContainer HeaderStyle(IContainer container)
                            {
                                return container.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                                    .PaddingVertical(5).PaddingHorizontal(5).Background("#1a1a2e");
                            }
                        });

                        foreach (var item in job.LineItems)
                        {
                            table.Cell().Element(CellStyle).Text(item.Description);
                            table.Cell().Element(CellStyle).Text(item.Category.ToString());
                            table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                            table.Cell().Element(CellStyle).Text(item.Unit ?? "ea");
                            table.Cell().Element(CellStyle).AlignRight().Text(FormatMoney(item.UnitPrice));
                            table.Cell().Element(CellStyle).AlignRight().Text(FormatMoney(item.TotalPrice));

                            static IContainer CellStyle(IContainer container)
                            {
                                return container.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingVertical(5).PaddingHorizontal(5);
                            }
                        }
                    });

                    col.Item().AlignRight().PaddingTop(20).Column(c =>
                    {
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().AlignRight().Text("Net Total: ");
                            r.ConstantItem(80).AlignRight().Text(FormatMoney(job.TotalRevenue));
                        });

                        var vatAmount = (long)(job.TotalRevenue * (job.User.DefaultVatRate / 100m));
                        c.Item().Row(r =>
                        {
                            r.RelativeItem().AlignRight().Text($"VAT ({job.User.DefaultVatRate}%): ");
                            r.ConstantItem(80).AlignRight().Text(FormatMoney(vatAmount));
                        });

                        c.Item().Row(r =>
                        {
                            r.RelativeItem().AlignRight().Text("Gross Total: ").FontSize(14).Bold();
                            r.ConstantItem(80).AlignRight().Text(FormatMoney(job.TotalRevenue + vatAmount)).FontSize(14).Bold();
                        });
                    });

                    if (!string.IsNullOrWhiteSpace(job.Description))
                    {
                        col.Item().PaddingTop(30).Column(c =>
                        {
                            c.Item().Text("Notes & Terms").SemiBold();
                            c.Item().PaddingTop(5).Text(job.Description);
                        });
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        });

        return document.GeneratePdf();
    }

    private string FormatMoney(long pence)
    {
        return (pence / 100.0).ToString("C2", new CultureInfo("en-GB"));
    }
}
