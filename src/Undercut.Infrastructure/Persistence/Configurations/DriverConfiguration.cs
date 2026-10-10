using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Undercut.Domain.Entities;

namespace Undercut.Infrastructure.Persistence.Configurations;

public class DriverConfiguration : IEntityTypeConfiguration<Driver>
{
    public void Configure(EntityTypeBuilder<Driver> b)
    {
        b.ToTable("Drivers");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Code).HasMaxLength(3).IsRequired();
        b.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Team).HasMaxLength(100).IsRequired();
        b.HasIndex(x => new { x.Season, x.Code }).IsUnique();
    }
}
