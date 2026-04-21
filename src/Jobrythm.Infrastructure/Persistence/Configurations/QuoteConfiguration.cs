using Jobrythm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobrythm.Infrastructure.Persistence.Configurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.QuoteNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>();

        builder.HasOne(x => x.Job)
            .WithOne(j => j.Quote)
            .HasForeignKey<Quote>(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.JobId);
        builder.HasIndex(x => x.Status);
    }
}
