using Microsoft.EntityFrameworkCore;
using NameGen.Core.Interfaces;
using NameGen.Core.Models;
using NameGen.Infrastructure.Data;

namespace NameGen.Infrastructure.Services;

public class HumanNameService : IHumanNameService
{
    private readonly AppDbContext _context;

    public HumanNameService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<HumanNameResponse> GenerateAsync(HumanNameRequest request)
    {
        var requestedCount = Math.Min(request.Count, 25);
        var genderEnum = ParseGender(request.Gender);
        var typeNormalized = request.Type.ToLower();
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

        var results = new List<HumanNameResult>();

        if (typeNormalized == "last")
        {
            var lastNames = await QueryNamesAsync(
                NameComponent.Last, Gender.Neutral,
                request.MinLength, request.MaxLength,
                includes, excludes, startsWith, notStartsWith,
                endsWith, notEndsWith);

            results = WeightedShuffle(lastNames, weightedNormalized, rng)
                .Take(requestedCount)
                .Select(n => new HumanNameResult { LastName = n.Name })
                .ToList();
        }
        else if (typeNormalized == "first")
        {
            var firstNames = await QueryNamesAsync(
                NameComponent.First, genderEnum,
                request.MinLength, request.MaxLength,
                includes, excludes, startsWith, notStartsWith,
                endsWith, notEndsWith);

            results = WeightedShuffle(firstNames, weightedNormalized, rng)
                .Take(requestedCount)
                .Select(n => new HumanNameResult
                {
                    FirstName = n.Name,
                    Gender    = n.Gender.ToString().ToLower()
                })
                .ToList();
        }
        else // full
        {
            var firstNames = await QueryNamesAsync(
                NameComponent.First, genderEnum,
                request.MinLength, request.MaxLength,
                includes, excludes, startsWith, notStartsWith,
                endsWith, notEndsWith);

            var lastNames = await QueryNamesAsync(
                NameComponent.Last, Gender.Neutral,
                request.MinLength, request.MaxLength,
                includes, excludes, startsWith, notStartsWith,
                endsWith, notEndsWith);

            var shuffledFirst = WeightedShuffle(firstNames, weightedNormalized, rng).ToList();
            var shuffledLast  = WeightedShuffle(lastNames, weightedNormalized, rng).ToList();

            int count = Math.Min(requestedCount,
                Math.Min(shuffledFirst.Count, shuffledLast.Count));

            for (int i = 0; i < count; i++)
            {
                var first = shuffledFirst[i];
                var last  = shuffledLast[i];
                results.Add(new HumanNameResult
                {
                    FirstName = first.Name,
                    LastName  = last.Name,
                    FullName  = $"{first.Name} {last.Name}",
                    Gender    = first.Gender.ToString().ToLower()
                });
            }

            if (request.MinFullLength.HasValue)
                results = results
                    .Where(r => r.FullName!.Length >= request.MinFullLength.Value)
                    .ToList();

            if (request.MaxFullLength.HasValue)
                results = results
                    .Where(r => r.FullName!.Length <= request.MaxFullLength.Value)
                    .ToList();
        }

        string? warning = null;
        if (results.Count < requestedCount)
            warning = $"Filters returned fewer results than requested. " +
                      $"{results.Count} of {requestedCount} requested names returned.";

        return new HumanNameResponse
        {
            Count          = results.Count,
            Type           = typeNormalized,
            Gender         = request.Gender.ToLower(),
            Results        = results,
            FiltersApplied = request,
            Warning        = warning
        };
    }

    /// <summary>
    /// Orders a name list by popularity weighting.
    /// weighted=common: lower rank numbers (more popular) appear first.
    /// weighted=rare:   higher rank numbers (less popular) appear first.
    /// weighted=none:   pure random shuffle.
    /// Names with null popularity are assigned a neutral middle rank.
    /// </summary>
    private static IEnumerable<HumanName> WeightedShuffle(
        List<HumanName> names,
        string weighted,
        Random rng)
    {
        if (weighted == "common")
            return names.OrderBy(n => n.Popularity ?? 50000)
                        .ThenBy(_ => rng.Next());

        if (weighted == "rare")
            return names.OrderByDescending(n => n.Popularity ?? 50000)
                        .ThenBy(_ => rng.Next());

        return names.OrderBy(_ => rng.Next());
    }

    private async Task<List<HumanName>> QueryNamesAsync(
        NameComponent component,
        Gender gender,
        int? minLength,
        int? maxLength,
        List<string> includes,
        List<string> excludes,
        List<string> startsWith,
        List<string> notStartsWith,
        List<string> endsWith,
        List<string> notEndsWith)
    {
        var query = _context.HumanNames
            .Where(n => n.Component == component)
            .AsQueryable();

        if (gender != Gender.Neutral)
            query = query.Where(n => n.Gender == gender);

        if (minLength.HasValue)
            query = query.Where(n => n.Name.Length >= minLength.Value);

        if (maxLength.HasValue)
            query = query.Where(n => n.Name.Length <= maxLength.Value);

        var nameList = await query.ToListAsync();

        if (includes.Any())
            nameList = nameList.Where(n =>
                includes.Any(v => n.Name.Contains(v,
                    StringComparison.OrdinalIgnoreCase))).ToList();

        if (excludes.Any())
            nameList = nameList.Where(n =>
                !excludes.Any(v => n.Name.Contains(v,
                    StringComparison.OrdinalIgnoreCase))).ToList();

        if (startsWith.Any())
            nameList = nameList.Where(n =>
                startsWith.Any(v => n.Name.StartsWith(v,
                    StringComparison.OrdinalIgnoreCase))).ToList();

        if (notStartsWith.Any())
            nameList = nameList.Where(n =>
                !notStartsWith.Any(v => n.Name.StartsWith(v,
                    StringComparison.OrdinalIgnoreCase))).ToList();

        if (endsWith.Any())
            nameList = nameList.Where(n =>
                endsWith.Any(v => n.Name.EndsWith(v,
                    StringComparison.OrdinalIgnoreCase))).ToList();

        if (notEndsWith.Any())
            nameList = nameList.Where(n =>
                !notEndsWith.Any(v => n.Name.EndsWith(v,
                    StringComparison.OrdinalIgnoreCase))).ToList();

        return nameList;
    }

    private static Gender ParseGender(string gender) =>
        gender.ToLower() switch
        {
            "male"   => Gender.Male,
            "female" => Gender.Female,
            _        => Gender.Neutral
        };

    private static List<string> SplitFilter(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? new List<string>()
            : value.Split(',', StringSplitOptions.RemoveEmptyEntries)
                   .Select(v => v.Trim())
                   .ToList();
}