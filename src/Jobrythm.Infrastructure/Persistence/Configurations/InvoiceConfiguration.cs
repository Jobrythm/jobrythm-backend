using Jobrythm.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Jobrythm.Infrastructure.Persistence.Configurations;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.InvoiceNumber).IsRequired().HasMaxLength(50);
        builder.Property(x => x.Status).HasConversion<string>();

        builder.HasOne(x => x.Job)
            .WithOne(j => j.Invoice)
            .HasForeignKey<Invoice>(x => x.JobId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.JobId);
        builder.HasIndex(x => x.Status);
    }
}
