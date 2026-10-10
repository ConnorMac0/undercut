namespace Undercut.Domain.Abstractions;

/// <summary>Marker for entities that belong to exactly one league. These get a global query filter.</summary>
public interface ILeagueScoped
{
    Guid LeagueId { get; set; }
}
