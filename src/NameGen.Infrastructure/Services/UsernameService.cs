using NameGen.Core.Interfaces;
using NameGen.Core.Models;
using NameGen.Infrastructure.Data.Seed;

namespace NameGen.Infrastructure.Services;

public class UsernameService : IUsernameService
{
    private const int MaxRetries = 200;

    public Task<UsernameResponse> GenerateAsync(UsernameRequest request)
    {
        var requestedCount = Math.Min(request.Count, 25);
        var styleNormalized = request.Style.ToLower();

        var rng = request.Seed.HasValue
            ? new Random(request.Seed.Value)
            : new Random();

        var includes      = SplitFilter(request.Includes);
        var excludes      = SplitFilter(request.Excludes);
        var startsWith    = SplitFilter(request.StartsWith);
        var notStartsWith = SplitFilter(request.NotStartsWith);
        var endsWith      = SplitFilter(request.EndsWith);
        var notEndsWith   = SplitFilter(request.NotEndsWith);

        var results = new List<UsernameResult>();
        int attempts = 0;

        while (results.Count < requestedCount && attempts < MaxRetries)
        {
            attempts++;

            var styleKey = ResolveStyle(styleNormalized, rng);
            var preset = UsernamePresets.Presets[styleKey];

            var username = BuildUsername(preset, styleKey, request.AllowNumbers,
                request.AllowSymbols, rng);

            if (!PassesFilters(username, request.MinLength, request.MaxLength,
                includes, excludes, startsWith, notStartsWith, endsWith, notEndsWith))
                continue;

            results.Add(new UsernameResult
            {
                Username = username,
                Style    = styleKey
            });
        }

        string? warning = null;
        if (results.Count < requestedCount)
            warning = $"Filters returned fewer results than requested. " +
                      $"{results.Count} of {requestedCount} requested names returned.";

        return Task.FromResult(new UsernameResponse
        {
            Count          = results.Count,
            Style          = styleNormalized,
            Results        = results,
            FiltersApplied = request,
            Warning        = warning
        });
    }

    private static string BuildUsername(
        UsernameStylePreset preset,
        string styleKey,
        bool allowNumbers,
        bool allowSymbols,
        Random rng)
    {
        var prefix = preset.Prefixes[rng.Next(preset.Prefixes.Count)];
        var suffix = preset.Suffixes[rng.Next(preset.Suffixes.Count)];
        var core = prefix + suffix;

        // Sweaty style: wrap with xX...Xx when symbols allowed
        if (styleKey == "sweaty" && allowSymbols)
            return $"xX{core}Xx";

        // Append number when allowed (roughly 50% chance to keep variety)
        if (allowNumbers && rng.Next(2) == 0)
            core += rng.Next(10, 999).ToString();

        return core;
    }

    private static string ResolveStyle(string style, Random rng)
    {
        var styles = UsernamePresets.Presets.Keys.ToList();
        return style == "random"
            ? styles[rng.Next(styles.Count)]
            : (UsernamePresets.Presets.ContainsKey(style)
                ? style
                : styles[0]);
    }

    private static bool PassesFilters(
        string name,
        int? minLength, int? maxLength,
        List<string> includes, List<string> excludes,
        List<string> startsWith, List<string> notStartsWith,
        List<string> endsWith, List<string> notEndsWith)
    {
        if (minLength.HasValue && name.Length < minLength.Value) return false;
        if (maxLength.HasValue && name.Length > maxLength.Value) return false;

        if (includes.Any() && !includes.Any(v =>
            name.Contains(v, StringComparison.OrdinalIgnoreCase))) return false;

        if (excludes.Any() && excludes.Any(v =>
            name.Contains(v, StringComparison.OrdinalIgnoreCase))) return false;

        if (startsWith.Any() && !startsWith.Any(v =>
            name.StartsWith(v, StringComparison.OrdinalIgnoreCase))) return false;

        if (notStartsWith.Any() && notStartsWith.Any(v =>
            name.StartsWith(v, StringComparison.OrdinalIgnoreCase))) return false;

        if (endsWith.Any() && !endsWith.Any(v =>
            name.EndsWith(v, StringComparison.OrdinalIgnoreCase))) return false;

        if (notEndsWith.Any() && notEndsWith.Any(v =>
            name.EndsWith(v, StringComparison.OrdinalIgnoreCase))) return false;

        return true;
    }

    private static List<string> SplitFilter(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? new List<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(v => v.Trim())
                   .ToList();
}