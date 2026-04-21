namespace NameGen.Core.Models;

public class HumanNameResponse
{
    public int Count { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public List<HumanNameResult> Results { get; set; } = new();
    public HumanNameRequest? FiltersApplied { get; set; }
    public string? Warning { get; set; }
}

public class HumanNameResult
{
    public string? FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? LastName { get; set; }
    public string? FullName { get; set; }
    public string? Gender { get; set; }
}