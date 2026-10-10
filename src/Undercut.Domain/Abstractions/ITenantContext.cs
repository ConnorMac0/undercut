namespace Undercut.Domain.Abstractions;

/// <summary>The league (tenant) the current request is operating in. Null = no tenant, so tenant-scoped queries return nothing.</summary>
public interface ITenantContext
{
    Guid? LeagueId { get; }
}
