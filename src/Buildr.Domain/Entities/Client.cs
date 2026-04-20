using Buildr.Domain.Common;

namespace Buildr.Domain.Entities;

public class Client : BaseEntity
{
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser User { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string? Address { get; set; }

    public ICollection<Job> Jobs { get; set; } = [];
}
