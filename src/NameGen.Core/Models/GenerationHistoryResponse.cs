namespace NameGen.Core.Models;

public class GenerationHistoryResult
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public object? Parameters { get; set; }
    public int ResultCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class GenerationHistoryListResponse
{
    public int Count { get; set; }
    public List<GenerationHistoryResult> Results { get; set; } = new();
}