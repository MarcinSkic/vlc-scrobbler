using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace VlcTracker.Service.Persistence.Entities.Configurations;

public class ScrobbleEntityConfiguration : IEntityTypeConfiguration<Scrobble>
{
    public void Configure(EntityTypeBuilder<Scrobble> builder)
    {
        builder.ToTable("Scrobbles", "tracker");

        builder.HasKey(s => s.Id);
        builder
            .Property(s => s.Id)
            .HasValueGenerator((_, _) => new StringValueGenerator())
            .ValueGeneratedOnAdd();

        builder.Property(s => s.Title).HasMaxLength(500).IsRequired(false);

        builder.Property(s => s.FileName).HasMaxLength(500).IsRequired();

        builder.Property(s => s.VideoDuration).IsRequired(false);

        builder.Property(s => s.ScrobbleDuration).IsRequired(false);

        builder.Property(s => s.Date).IsRequired(false);
        
        builder.Property(s => s.CreatedAt).IsRequired();

        builder.HasIndex(s => s.Title);
        builder.HasIndex(s => s.FileName);
    }
}
