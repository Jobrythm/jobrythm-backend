using Jobrythm.Domain.Common;

namespace Jobrythm.Domain.Entities;

public class NumberSequence : BaseEntity
{
    public string UserId { get; set; } = string.Empty;

    /// <summary>Prefix such as "QT" for quotes or "INV" for invoices.</summary>
    public string Prefix { get; set; } = string.Empty;

    public int LastNumber { get; set; } = 0;
}
