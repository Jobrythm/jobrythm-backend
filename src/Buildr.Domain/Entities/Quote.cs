using Buildr.Domain.Common;
using Buildr.Domain.Enums;

namespace Buildr.Domain.Entities;

public class Quote : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string QuoteNumber { get; set; } = string.Empty;
    public QuoteStatus Status { get; set; } = QuoteStatus.Draft;
    public DateTime? ValidUntil { get; set; }

    public string? Notes { get; set; }
    public string? Terms { get; set; }

    public long TotalNet { get; set; }
    public int VatRate { get; set; }
    public long VatAmount { get; set; }
    public long TotalGross { get; set; }

    public DateTime? SentAt { get; set; }
    public DateTime? AcceptedAt { get; set; }
}
