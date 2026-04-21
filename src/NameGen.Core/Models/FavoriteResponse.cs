namespace NameGen.Core.Models;

public class FavoriteResult
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Gender { get; set; }
    public string? Style { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class FavoriteListResponse
{
    public int Count { get; set; }
    public List<FavoriteResult> Results { get; set; } = new();
}