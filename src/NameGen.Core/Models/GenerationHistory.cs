namespace NameGen.Core.Models;

public class GenerationHistory
{
    public int Id { get; set; }
    public GenerationType Type { get; set; }
    public string ParametersJson { get; set; } = string.Empty;
    public string ResultsJson { get; set; } = string.Empty;
    public int ResultCount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}