namespace Undercut.Domain.Entities;

public class Result
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid RaceId { get; set; }
    public int Position { get; set; }
    public Guid DriverId { get; set; }

    public Race? Race { get; set; }
    public Driver? Driver { get; set; }
}
