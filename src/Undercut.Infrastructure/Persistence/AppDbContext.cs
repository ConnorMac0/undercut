using Microsoft.EntityFrameworkCore;
using Undercut.Domain.Abstractions;
using Undercut.Domain.Entities;

namespace Undercut.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    private readonly ITenantContext _tenant;

    public AppDbContext(DbContextOptions<AppDbContext> options, ITenantContext tenant)
        : base(options) => _tenant = tenant;

    public DbSet<User> Users => Set<User>();
    public DbSet<League> Leagues => Set<League>();
    public DbSet<Membership> Memberships => Set<Membership>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Race> Races => Set<Race>();
    public DbSet<Pick> Picks => Set<Pick>();
    public DbSet<Result> Results => Set<Result>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // _tenant is read per DbContext instance, i.e. per request.
        // LeagueId == null matches nothing: no tenant means no rows (safe failure mode).
        b.Entity<Membership>().HasQueryFilter(m => m.LeagueId == _tenant.LeagueId);
        b.Entity<Pick>().HasQueryFilter(p => p.LeagueId == _tenant.LeagueId);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<ILeagueScoped>())
        {
            if (entry.State == EntityState.Added && entry.Entity.LeagueId == Guid.Empty)
            {
                entry.Entity.LeagueId = _tenant.LeagueId
                    ?? throw new InvalidOperationException("No tenant set.");
            }
            else if (entry.State is EntityState.Added or EntityState.Modified
                     && entry.Entity.LeagueId != _tenant.LeagueId)
            {
                throw new InvalidOperationException("Cross-tenant write blocked.");
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
