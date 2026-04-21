namespace NameGen.Core.Models;

public class HumanName
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public NameComponent Component { get; set; }
    public Gender Gender { get; set; }
    public string? Origin { get; set; }
    public int? Popularity { get; set; }
    public int? Era { get; set; }
}