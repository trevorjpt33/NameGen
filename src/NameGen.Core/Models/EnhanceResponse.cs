namespace NameGen.Core.Models;

public class EnhanceResponse
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public EnhancementDetail? Enhancement { get; set; }
    public string? Message { get; set; }
}

public class EnhancementDetail
{
    public string? RefinedName { get; set; }
    public string? LoreBlurb { get; set; }
}