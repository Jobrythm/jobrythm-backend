using Jobrythm.Domain.Common;
using Jobrythm.Domain.Enums;

namespace Jobrythm.Domain.Entities;

public class Invoice : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string InvoiceNumber { get; set; } = string.Empty;
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public DateTime? DueDate { get; set; }

    public string? Notes { get; set; }
    public string? Terms { get; set; }

    public long TotalNet { get; set; }
    public int VatRate { get; set; }
    public long VatAmount { get; set; }
    public long TotalGross { get; set; }

    public DateTime? SentAt { get; set; }
    public DateTime? PaidAt { get; set; }
}
