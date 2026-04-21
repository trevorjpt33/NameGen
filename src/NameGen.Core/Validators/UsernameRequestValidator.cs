using FluentValidation;
using NameGen.Core.Models;

namespace NameGen.Core.Validators;

public class UsernameRequestValidator : AbstractValidator<UsernameRequest>
{
    private static readonly string[] ValidStyles   = ["sweaty", "clean", "retro", "fantasy", "random"];
    private static readonly string[] ValidWeighted = ["none", "common", "rare"];

    public UsernameRequestValidator()
    {
        RuleFor(x => x.Count)
            .InclusiveBetween(1, 25)
            .WithMessage("count must be between 1 and 25.");

        RuleFor(x => x.Style)
            .Must(s => ValidStyles.Contains(s.ToLower()))
            .WithMessage("style must be one of: sweaty, clean, retro, fantasy, random.");

        RuleFor(x => x.Weighted)
            .Must(w => ValidWeighted.Contains(w.ToLower()))
            .WithMessage("weighted must be one of: none, common, rare.");

        RuleFor(x => x)
            .Must(x => x.MinLength == null || x.MaxLength == null || x.MinLength <= x.MaxLength)
            .WithMessage("minLength cannot be greater than maxLength.");

        RuleFor(x => x)
            .Must(x => !HasOverlap(x.StartsWith, x.NotStartsWith))
            .WithMessage("startsWith and notStartsWith cannot contain the same values.");

        RuleFor(x => x)
            .Must(x => !HasOverlap(x.EndsWith, x.NotEndsWith))
            .WithMessage("endsWith and notEndsWith cannot contain the same values.");

        RuleFor(x => x)
            .Must(x => !HasOverlap(x.Includes, x.Excludes))
            .WithMessage("includes and excludes cannot contain the same values.");
    }

    private static bool HasOverlap(string? a, string? b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
        var setA = a.Split(',').Select(v => v.Trim().ToLower()).ToHashSet();
        var setB = b.Split(',').Select(v => v.Trim().ToLower()).ToHashSet();
        return setA.Overlaps(setB);
    }
}