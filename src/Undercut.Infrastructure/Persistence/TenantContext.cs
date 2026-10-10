using Undercut.Domain.Abstractions;

namespace Undercut.Infrastructure.Persistence;

/// <summary>Scoped per request. Set by the membership-check middleware once the caller is verified as a member.</summary>
public class TenantContext : ITenantContext
{
    public Guid? LeagueId { get; private set; }

    public void Set(Guid leagueId) => LeagueId = leagueId;
}
