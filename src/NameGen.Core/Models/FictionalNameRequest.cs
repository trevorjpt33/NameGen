namespace NameGen.Core.Models;

public class FictionalNameRequest
{
    public string Style { get; set; } = "random";
    public string Type { get; set; } = "full";
    public int Count { get; set; } = 5;
    public int? Seed { get; set; }
    public string Weighted { get; set; } = "none";
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public int? MinFullLength { get; set; }
    public int? MaxFullLength { get; set; }
    public string? Includes { get; set; }
    public string? Excludes { get; set; }
    public string? StartsWith { get; set; }
    public string? NotStartsWith { get; set; }
    public string? EndsWith { get; set; }
    public string? NotEndsWith { get; set; }
}