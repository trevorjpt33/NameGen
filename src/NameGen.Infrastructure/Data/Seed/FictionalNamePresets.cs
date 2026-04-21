namespace NameGen.Infrastructure.Data.Seed;

public static class FictionalNamePresets
{
    public static readonly Dictionary<string, StylePreset> Presets = new()
    {
        ["elvish"] = new StylePreset
        {
            Prefixes = ["Ara", "Celi", "Elo", "Fae", "Gala", "Itha", "Liri", "Miri", "Syla", "Thari"],
            Middles  = ["an", "dri", "el", "en", "ion", "las", "mir", "rin", "ven", "wyn"],
            Suffixes = ["dor", "el", "iel", "ion", "las", "mir", "niel", "ril", "wyn", "vel"]
        },
        ["nordic"] = new StylePreset
        {
            Prefixes = ["Arn", "Bjorn", "Dag", "Eirik", "Gunnar", "Hald", "Ivar", "Leif", "Sven", "Thor"],
            Middles  = ["al", "dren", "gar", "mar", "old", "rik", "ulf", "var", "vik", "wald"],
            Suffixes = ["dren", "gar", "heim", "mar", "old", "rik", "ulf", "var", "vik", "wald"]
        },
        ["villainous"] = new StylePreset
        {
            Prefixes = ["Bal", "Drak", "Grim", "Kael", "Mal", "Mor", "Rax", "Sha", "Vel", "Vex"],
            Middles  = ["a", "ak", "ar", "eth", "ix", "kor", "mor", "oth", "rak", "ur"],
            Suffixes = ["ach", "ar", "eth", "gor", "ix", "kar", "mor", "oth", "thar", "vor"]
        }
    };
}

public class StylePreset
{
    public List<string> Prefixes { get; set; } = new();
    public List<string> Middles { get; set; } = new();
    public List<string> Suffixes { get; set; } = new();
}