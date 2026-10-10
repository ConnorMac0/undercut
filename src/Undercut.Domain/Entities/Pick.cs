using Undercut.Domain.Abstractions;

namespace Undercut.Domain.Entities;

public class Pick : ILeagueScoped
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid LeagueId { get; set; }   // denormalized on purpose so the query filter needs no join
    public Guid RaceId { get; set; }
    public Guid UserId { get; set; }

    public Guid P1DriverId { get; set; }
    public Guid P2DriverId { get; set; }
    public Guid P3DriverId { get; set; }

    public byte[] RowVersion { get; set; } = Array.Empty<byte>();

    public League? League { get; set; }
    public Race? Race { get; set; }
    public User? User { get; set; }
    public Driver? P1Driver { get; set; }
    public Driver? P2Driver { get; set; }
    public Driver? P3Driver { get; set; }
}
