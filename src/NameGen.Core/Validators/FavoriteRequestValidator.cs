using FluentValidation;
using NameGen.Core.Models;

namespace NameGen.Core.Validators;

public class FavoriteRequestValidator : AbstractValidator<FavoriteRequest>
{
    private static readonly string[] ValidTypes   = ["human", "fictional", "username"];
    private static readonly string[] ValidGenders = ["male", "female", "neutral"];

    public FavoriteRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("name is required.")
            .MaximumLength(200)
            .WithMessage("name cannot exceed 200 characters.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .WithMessage("type is required.")
            .Must(t => ValidTypes.Contains(t.ToLower()))
            .WithMessage("type must be one of: human, fictional, username.");

        RuleFor(x => x.Gender)
            .Must(g => g == null || ValidGenders.Contains(g.ToLower()))
            .WithMessage("gender must be one of: male, female, neutral.");
    }
}