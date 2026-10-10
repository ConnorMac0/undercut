using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Undercut.Domain.Entities;

namespace Undercut.Infrastructure.Persistence.Configurations;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> b)
    {
        b.ToTable("Memberships");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Role).HasConversion<string>().HasMaxLength(20).IsRequired();

        // Real defense against duplicate joins under concurrency.
        b.HasIndex(x => new { x.LeagueId, x.UserId }).IsUnique();
        b.HasIndex(x => x.UserId); // "my leagues" lookup

        b.HasOne(x => x.League).WithMany(l => l.Memberships)
            .HasForeignKey(x => x.LeagueId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.User).WithMany(u => u.Memberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
