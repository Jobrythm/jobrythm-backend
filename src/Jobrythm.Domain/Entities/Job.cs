using Jobrythm.Domain.Common;
using Jobrythm.Domain.Enums;

namespace Jobrythm.Domain.Entities;

public class Job : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public Guid ClientId { get; set; }
    public Client Client { get; set; } = null!;

    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JobStatus Status { get; set; } = JobStatus.Draft;

    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public ICollection<LineItem> LineItems { get; set; } = [];
    public Quote? Quote { get; set; }
    public Invoice? Invoice { get; set; }

    public decimal TotalCost => LineItems.Sum(li => li.TotalCost);
    public decimal TotalRevenue => LineItems.Sum(li => li.TotalPrice);
    public decimal MarginPercent => TotalRevenue == 0
        ? 0
        : Math.Round((TotalRevenue - TotalCost) / TotalRevenue * 100, 2);
}
