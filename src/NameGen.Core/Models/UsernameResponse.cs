namespace NameGen.Core.Models;

public class UsernameResult
{
    public string Username { get; set; } = string.Empty;
    public string Style { get; set; } = string.Empty;
}

public class UsernameResponse
{
    public int Count { get; set; }
    public string Style { get; set; } = string.Empty;
    public List<UsernameResult> Results { get; set; } = new();
    public UsernameRequest? FiltersApplied { get; set; }
    public string? Warning { get; set; }
}