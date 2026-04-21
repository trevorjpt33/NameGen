namespace NameGen.Core.Models;

public class FictionalNameResult
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Style { get; set; }
}

public class FictionalNameResponse
{
    public int Count { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
    public List<FictionalNameResult> Results { get; set; } = new();
    public FictionalNameRequest? FiltersApplied { get; set; }
    public string? Warning { get; set; }
}