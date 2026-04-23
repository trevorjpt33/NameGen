using FluentValidation;
using NameGen.Core.Models;

namespace NameGen.Core.Validators;

public class EnhanceRequestValidator : AbstractValidator<EnhanceRequest>
{
    private static readonly string[] ValidTypes = ["human", "fictional", "username"];

    public EnhanceRequestValidator()
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
    }
}