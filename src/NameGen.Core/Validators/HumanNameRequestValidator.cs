using FluentValidation;
using NameGen.Core.Models;

namespace NameGen.Core.Validators;

public class HumanNameRequestValidator : AbstractValidator<HumanNameRequest>
{
    private static readonly string[] ValidTypes    = ["first", "middle", "last", "full"];
    private static readonly string[] ValidGenders  = ["male", "female", "neutral"];
    private static readonly string[] ValidWeighted = ["none", "common", "rare"];

    public HumanNameRequestValidator()
    {
        RuleFor(x => x.Count)
            .InclusiveBetween(1, 25)
            .WithMessage("count must be between 1 and 25.");

        RuleFor(x => x.Type)
            .Must(t => ValidTypes.Contains(t.ToLower()))
            .WithMessage("type must be one of: first, middle, last, full.");

        RuleFor(x => x.Gender)
            .Must(g => ValidGenders.Contains(g.ToLower()))
            .WithMessage("gender must be one of: male, female, neutral.");

        RuleFor(x => x.Weighted)
            .Must(w => ValidWeighted.Contains(w.ToLower()))
            .WithMessage("weighted must be one of: none, common, rare.");

        RuleFor(x => x)
            .Must(x => x.MinLength == null || x.MaxLength == null || x.MinLength <= x.MaxLength)
            .WithMessage("minLength cannot be greater than maxLength.");

        RuleFor(x => x)
            .Must(x => x.MinFullLength == null || x.MaxFullLength == null || x.MinFullLength <= x.MaxFullLength)
            .WithMessage("minFullLength cannot be greater than maxFullLength.");

        RuleFor(x => x)
            .Must(x => x.MinFullLength == null || x.MinLength == null || x.MinFullLength >= x.MinLength)
            .WithMessage("minFullLength cannot be less than minLength.");

        RuleFor(x => x)
            .Must(x => x.MaxFullLength == null || x.MaxLength == null || x.MaxFullLength >= x.MaxLength)
            .WithMessage("maxFullLength cannot be less than maxLength.");

        RuleFor(x => x)
            .Must(x => !HasOverlap(x.StartsWith, x.NotStartsWith))
            .WithMessage("startsWith and notStartsWith cannot contain the same values.");

        RuleFor(x => x)
            .Must(x => !HasOverlap(x.EndsWith, x.NotEndsWith))
            .WithMessage("endsWith and notEndsWith cannot contain the same values.");

        RuleFor(x => x)
            .Must(x => !HasOverlap(x.Includes, x.Excludes))
            .WithMessage("includes and excludes cannot contain the same values.");

        RuleFor(x => x)
            .Must(x => x.MinFullLength == null || x.MaxFullLength == null ||
                       x.Type.ToLower() == "full")
            .WithMessage("minFullLength and maxFullLength can only be used with type=full.");
    }

    private static bool HasOverlap(string? a, string? b)
    {
        if (string.IsNullOrWhiteSpace(a) || string.IsNullOrWhiteSpace(b)) return false;
        var setA = a.Split(',').Select(v => v.Trim().ToLower()).ToHashSet();
        var setB = b.Split(',').Select(v => v.Trim().ToLower()).ToHashSet();
        return setA.Overlaps(setB);
    }
}