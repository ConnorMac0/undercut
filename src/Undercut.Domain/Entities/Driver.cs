namespace Undercut.Domain.Entities;

public class Driver
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Code { get; set; } = string.Empty;      // e.g. "VER"
    public string FullName { get; set; } = string.Empty;
    public string Team { get; set; } = string.Empty;
    public int Season { get; set; }
}
