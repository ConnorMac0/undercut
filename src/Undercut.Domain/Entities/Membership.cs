using Undercut.Domain.Abstractions;
using Undercut.Domain.Enums;

namespace Undercut.Domain.Entities;

public class Membership : ILeagueScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LeagueId { get; set; }
    public Guid UserId { get; set; }
    public LeagueRole Role { get; set; } = LeagueRole.Player;
    public DateTimeOffset JoinedAt { get; set; } = DateTimeOffset.UtcNow;

    public League? League { get; set; }
    public User? User { get; set; }
}
