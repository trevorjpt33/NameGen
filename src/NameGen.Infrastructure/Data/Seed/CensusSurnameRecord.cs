using CsvHelper.Configuration.Attributes;

namespace NameGen.Infrastructure.Data.Seed;

public class CensusSurnameRecord
{
    [Name("name")]
    public string Name { get; set; } = string.Empty;

    [Name("rank")]
    public int Rank { get; set; }
}