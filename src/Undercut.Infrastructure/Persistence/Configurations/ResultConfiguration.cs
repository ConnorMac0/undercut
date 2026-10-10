using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Undercut.Domain.Entities;

namespace Undercut.Infrastructure.Persistence.Configurations;

public class ResultConfiguration : IEntityTypeConfiguration<Result>
{
    public void Configure(EntityTypeBuilder<Result> b)
    {
        b.ToTable("Results");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        b.HasIndex(x => new { x.RaceId, x.Position }).IsUnique();
        b.HasIndex(x => new { x.RaceId, x.DriverId }).IsUnique();

        b.HasOne(x => x.Race).WithMany()
            .HasForeignKey(x => x.RaceId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.Driver).WithMany()
            .HasForeignKey(x => x.DriverId).OnDelete(DeleteBehavior.Restrict);
    }
}
