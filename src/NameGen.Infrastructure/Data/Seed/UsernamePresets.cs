namespace NameGen.Infrastructure.Data.Seed;

public static class UsernamePresets
{
    public static readonly Dictionary<string, UsernameStylePreset> Presets = new()
    {
        ["sweaty"] = new UsernameStylePreset
        {
            Prefixes = ["Dark", "Shadow", "Vex", "Blaze", "Frost", "Storm", "Rage", "Nite", "Void", "Apex"],
            Suffixes = ["Slayer", "Hunter", "Killer", "Lord", "Master", "Reaper", "Blade", "Shot", "King", "God"]
        },
        ["clean"] = new UsernameStylePreset
        {
            Prefixes = ["Vex", "Kael", "Nyx", "Zyn", "Ryn", "Lux", "Dex", "Flux", "Hex", "Axe"],
            Suffixes = ["", "en", "on", "is", "ar", "ex", "ix", "yn", "us", "or"]
        },
        ["retro"] = new UsernameStylePreset
        {
            Prefixes = ["Pr0", "L33t", "xX", "D4rk", "Sn1p", "Fr4g", "Pwn", "N00b", "H4x", "G4m"],
            Suffixes = ["Gamer", "Shot", "Hax", "Star", "King", "Ninja", "Pro", "Ace", "Bot", "Ster"]
        },
        ["fantasy"] = new UsernameStylePreset
        {
            Prefixes = ["Shadow", "Dusk", "Storm", "Iron", "Ember", "Frost", "Thunder", "Silver", "Dark", "Ash"],
            Suffixes = ["mere", "bane", "forge", "blade", "heart", "moor", "vale", "fell", "born", "sworn"]
        }
    };
}

public class UsernameStylePreset
{
    public List<string> Prefixes { get; set; } = new();
    public List<string> Suffixes { get; set; } = new();
}