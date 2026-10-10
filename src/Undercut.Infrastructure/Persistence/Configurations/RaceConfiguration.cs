using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Undercut.Domain.Entities;

namespace Undercut.Infrastructure.Persistence.Configurations;

public class RaceConfiguration : IEntityTypeConfiguration<Race>
{
    public void Configure(EntityTypeBuilder<Race> b)
    {
        b.ToTable("Races");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.Season, x.Round }).IsUnique();
    }
}
