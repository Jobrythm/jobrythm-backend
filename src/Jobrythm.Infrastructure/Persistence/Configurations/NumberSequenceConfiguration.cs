using Jobrythm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobrythm.Infrastructure.Persistence.Configurations;

public class NumberSequenceConfiguration : IEntityTypeConfiguration<NumberSequence>
{
    public void Configure(EntityTypeBuilder<NumberSequence> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Prefix).IsRequired().HasMaxLength(10);

        builder.HasIndex(x => new { x.UserId, x.Prefix }).IsUnique();
    }
}
