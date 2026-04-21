namespace NameGen.Core.Models;

public class FavoriteRequest
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? Style { get; set; }
}