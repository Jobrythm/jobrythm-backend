using Jobrythm.Domain.Common;
using Jobrythm.Domain.Enums;

namespace Jobrythm.Domain.Entities;

public class LineItem : BaseEntity
{
    public Guid JobId { get; set; }
    public Job Job { get; set; } = null!;

    public string Description { get; set; } = string.Empty;
    public LineItemCategory Category { get; set; }
    public decimal Quantity { get; set; }
    public string? Unit { get; set; }

    /// <summary>Cost per unit stored as pence/cents (integer).</summary>
    public long UnitCost { get; set; }

    /// <summary>Price per unit stored as pence/cents (integer).</summary>
    public long UnitPrice { get; set; }

    public long TotalCost => (long)(Quantity * UnitCost);
    public long TotalPrice => (long)(Quantity * UnitPrice);
    public decimal MarginPercent => TotalPrice == 0
        ? 0
        : Math.Round((TotalPrice - TotalCost) / (decimal)TotalPrice * 100, 2);
}
