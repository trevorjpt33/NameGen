using NameGen.Core.Interfaces;
using NameGen.Core.Models;
using NameGen.Infrastructure.Data.Seed;

namespace NameGen.Infrastructure.Services;

public class FictionalNameService : IFictionalNameService
{
    private const int MaxRetries = 200;

    public Task<FictionalNameResponse> GenerateAsync(FictionalNameRequest request)
    {
        var requestedCount = Math.Min(request.Count, 25);
        var styleKey = ResolveStyle(request.Style);
        var typeNormalized = request.Type.ToLower();

        var rng = request.Seed.HasValue
            ? new Random(request.Seed.Value)
            : new Random();

        var preset = FictionalNamePresets.Presets[styleKey];

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

            string? firstName = null;
            string? lastName = null;

            if (typeNormalized != "last")
                firstName = BuildName(preset, rng);

            if (typeNormalized != "first")
                lastName = BuildName(preset, rng);

            // Apply per-component filters
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

            // Apply full name length filters
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
            Count         = results.Count,
            Type          = typeNormalized,
            Style         = styleKey,
            Results       = results,
            FiltersApplied = request,
            Warning       = warning
        });
    }

    private static string BuildName(StylePreset preset, Random rng)
    {
        var prefix = preset.Prefixes[rng.Next(preset.Prefixes.Count)];
        var middle = preset.Middles[rng.Next(preset.Middles.Count)];
        var suffix = preset.Suffixes[rng.Next(preset.Suffixes.Count)];
        return char.ToUpper(prefix[0]) + (prefix[1..] + middle + suffix).ToLower();
    }

    private static string ResolveStyle(string style)
    {
        var styles = FictionalNamePresets.Presets.Keys.ToList();
        return style.ToLower() == "random"
            ? styles[new Random().Next(styles.Count)]
            : (FictionalNamePresets.Presets.ContainsKey(style.ToLower())
                ? style.ToLower()
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