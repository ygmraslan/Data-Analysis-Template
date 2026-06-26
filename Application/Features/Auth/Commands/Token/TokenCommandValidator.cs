using FluentValidation;

namespace DataAnalysis.Application.Features.Auth.Commands.Token;

public class TokenCommandValidator : AbstractValidator<TokenCommand>
{
    public TokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Refresh token is required.");
    }
}