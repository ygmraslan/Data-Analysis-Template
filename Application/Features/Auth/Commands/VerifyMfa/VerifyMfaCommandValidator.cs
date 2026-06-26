using FluentValidation;

namespace DataAnalysis.Application.Features.Auth.Commands.VerifyMfa;

public class VerifyMfaCommandValidator : AbstractValidator<VerifyMfaCommand>
{
    public VerifyMfaCommandValidator()
    {
        RuleFor(x => x.MfaToken)
            .NotEmpty().WithMessage("MFA token is required.");

        RuleFor(x => x.MfaCode)
            .NotEmpty().WithMessage("MFA code is required.")
            .Length(6).WithMessage("MFA code must be 6 digits.")
            .Matches("^[0-9]+$").WithMessage("MFA code must contain only digits.");
    }
}