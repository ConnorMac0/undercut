namespace Undercut.Domain.Entities;

public class Race
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public int Season { get; set; }
    public int Round { get; set; }
    public string Name { get; set; } = string.Empty;

    /// <summary>Picks lock at this instant (server-enforced).</summary>
    public DateTimeOffset QualifyingStartUtc { get; set; }
    public DateTimeOffset RaceStartUtc { get; set; }
}
