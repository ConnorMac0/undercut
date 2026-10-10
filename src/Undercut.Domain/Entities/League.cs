namespace Undercut.Domain.Entities;

/// <summary>The tenant. Not itself query-filtered: it IS the tenant boundary.</summary>
public class League
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public string InviteCode { get; set; } = string.Empty;
    public Guid CreatedByUserId { get; set; }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

    public User? CreatedBy { get; set; }
    public List<Membership> Memberships { get; set; } = new();
}
