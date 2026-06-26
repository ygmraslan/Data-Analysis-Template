using FluentValidation;

namespace DataAnalysis.Application.Features.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommandValidator : AbstractValidator<ResetUserPasswordCommand>
{
    public ResetUserPasswordCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID is required.");
    }
}