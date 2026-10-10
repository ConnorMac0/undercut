using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Undercut.Domain.Entities;

namespace Undercut.Infrastructure.Persistence.Configurations;

public class PickConfiguration : IEntityTypeConfiguration<Pick>
{
    public void Configure(EntityTypeBuilder<Pick> b)
    {
        b.ToTable("Picks");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();

        // Optimistic concurrency: two edits near the lock deadline can't silently overwrite each other.
        b.Property(x => x.RowVersion).IsRowVersion();

        // One pick per player per race per league.
        b.HasIndex(x => new { x.LeagueId, x.RaceId, x.UserId }).IsUnique();

        b.HasOne(x => x.League).WithMany()
            .HasForeignKey(x => x.LeagueId).OnDelete(DeleteBehavior.Cascade);

        // Everything else is Restrict: three FKs to Drivers plus User/Race would otherwise
        // trigger SQL Server "multiple cascade paths" errors.
        b.HasOne(x => x.Race).WithMany()
            .HasForeignKey(x => x.RaceId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.User).WithMany()
            .HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.P1Driver).WithMany()
            .HasForeignKey(x => x.P1DriverId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.P2Driver).WithMany()
            .HasForeignKey(x => x.P2DriverId).OnDelete(DeleteBehavior.Restrict);
        b.HasOne(x => x.P3Driver).WithMany()
            .HasForeignKey(x => x.P3DriverId).OnDelete(DeleteBehavior.Restrict);

        // "Three distinct drivers" is a business rule: enforce it in the service in week 2,
        // and optionally add a CHECK constraint here.
    }
}
