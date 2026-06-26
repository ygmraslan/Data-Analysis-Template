using FluentValidation;

namespace DataAnalysis.Application.Features.Users.Commands.ResetUserMfa;

public class ResetUserMfaCommandValidator : AbstractValidator<ResetUserMfaCommand>
{
    public ResetUserMfaCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID is required.");
    }
}