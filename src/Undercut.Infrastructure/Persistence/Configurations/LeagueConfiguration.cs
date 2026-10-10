using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Undercut.Domain.Entities;

namespace Undercut.Infrastructure.Persistence.Configurations;

public class LeagueConfiguration : IEntityTypeConfiguration<League>
{
    public void Configure(EntityTypeBuilder<League> b)
    {
        b.ToTable("Leagues");
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.Property(x => x.InviteCode).HasMaxLength(16).IsRequired();
        b.HasIndex(x => x.InviteCode).IsUnique();

        // Restrict on User FKs everywhere: avoids SQL Server "multiple cascade paths" errors.
        b.HasOne(x => x.CreatedBy).WithMany()
            .HasForeignKey(x => x.CreatedByUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
