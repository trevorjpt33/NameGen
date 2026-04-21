namespace NameGen.Core.Models;

public class Favorite
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public GenerationType Type { get; set; }
    public Gender? Gender { get; set; }
    public string? Style { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}