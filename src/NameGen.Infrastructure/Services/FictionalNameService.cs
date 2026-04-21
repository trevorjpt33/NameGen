using NameGen.Core.Interfaces;
using NameGen.Core.Models;
using NameGen.Infrastructure.Data.Seed;

namespace NameGen.Infrastructure.Services;

public class FictionalNameService : IFictionalNameService
{
    private const int MaxRetries = 200;

    public Task<FictionalNameResponse> GenerateAsync(FictionalNameRequest request)
    {
        var requestedCount    = Math.Min(request.Count, 25);
        var styleNormalized   = request.Style.ToLower();
        var typeNormalized    = request.Type.ToLower();
        var weightedNormalized = request.Weighted.ToLower();

        var rng = request.Seed.HasValue
            ? new Random(request.Seed.Value)
            : new Random();

        var includes      = SplitFilter(request.Includes);
        var excludes      = SplitFilter(request.Excludes);
        var startsWith    = SplitFilter(request.StartsWith);
        var notStartsWith = SplitFilter(request.NotStartsWith);
        var endsWith      = SplitFilter(request.EndsWith);
        var notEndsWith   = SplitFilter(request.NotEndsWith);

        var results = new List<FictionalNameResult>();
        int attempts = 0;

        while (results.Count < requestedCount && attempts < MaxRetries)
        {
            attempts++;

            var styleKey = ResolveStyle(styleNormalized, rng);
            var preset   = FictionalNamePresets.Presets[styleKey];

            string? firstName = null;
            string? lastName  = null;

            if (typeNormalized != "last")
                firstName = BuildName(preset, weightedNormalized, rng);

            if (typeNormalized != "first")
                lastName = BuildName(preset, weightedNormalized, rng);

            if (firstName != null && !PassesFilters(firstName,
                request.MinLength, request.MaxLength,
                includes, excludes, startsWith, notStartsWith,
                endsWith, notEndsWith))
                continue;

            if (lastName != null && !PassesFilters(lastName,
                request.MinLength, request.MaxLength,
                includes, excludes, startsWith, notStartsWith,
                endsWith, notEndsWith))
                continue;

            var fullName = typeNormalized == "full"
                ? $"{firstName} {lastName}"
                : firstName ?? lastName;

            if (typeNormalized == "full")
            {
                if (request.MinFullLength.HasValue && fullName!.Length < request.MinFullLength.Value)
                    continue;
                if (request.MaxFullLength.HasValue && fullName!.Length > request.MaxFullLength.Value)
                    continue;
            }

            results.Add(new FictionalNameResult
            {
                FirstName = firstName,
                LastName  = lastName,
                FullName  = typeNormalized == "full" ? fullName : null,
                Style     = styleKey
            });
        }

        string? warning = null;
        if (results.Count < requestedCount)
            warning = $"Filters returned fewer results than requested. " +
                      $"{results.Count} of {requestedCount} requested names returned.";

        return Task.FromResult(new FictionalNameResponse
        {
            Count          = results.Count,
            Type           = typeNormalized,
            Style          = styleNormalized,
            Results        = results,
            FiltersApplied = request,
            Warning        = warning
        });
    }

    /// <summary>
    /// Builds a name by chaining a prefix, middle, and suffix from the preset.
    /// weighted=common biases toward the front of each syllable list (more familiar sounds).
    /// weighted=rare biases toward the back of each syllable list (more unusual sounds).
    /// </summary>
    private static string BuildName(StylePreset preset, string weighted, Random rng)
    {
        var prefix = PickWeighted(preset.Prefixes, weighted, rng);
        var middle = PickWeighted(preset.Middles, weighted, rng);
        var suffix = PickWeighted(preset.Suffixes, weighted, rng);
        return char.ToUpper(prefix[0]) + (prefix[1..] + middle + suffix).ToLower();
    }

    /// <summary>
    /// Picks an item from a list using weighted index selection.
    /// common: triangular distribution biased toward index 0.
    /// rare:   triangular distribution biased toward the last index.
    /// none:   uniform random.
    /// </summary>
    private static string PickWeighted(List<string> list, string weighted, Random rng)
    {
        if (list.Count == 0) return string.Empty;

        if (weighted == "common")
        {
            // Take the minimum of two random indexes — biases toward lower indexes
            int a = rng.Next(list.Count);
            int b = rng.Next(list.Count);
            return list[Math.Min(a, b)];
        }

        if (weighted == "rare")
        {
            // Take the maximum of two random indexes — biases toward higher indexes
            int a = rng.Next(list.Count);
            int b = rng.Next(list.Count);
            return list[Math.Max(a, b)];
        }

        return list[rng.Next(list.Count)];
    }

    private static string ResolveStyle(string style, Random rng)
    {
        var styles = FictionalNamePresets.Presets.Keys.ToList();
        return style == "random"
            ? styles[rng.Next(styles.Count)]
            : (FictionalNamePresets.Presets.ContainsKey(style)
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