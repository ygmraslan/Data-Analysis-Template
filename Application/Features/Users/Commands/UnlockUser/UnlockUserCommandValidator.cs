using FluentValidation;

namespace DataAnalysis.Application.Features.Users.Commands.UnlockUser;

public class UnlockUserCommandValidator : AbstractValidator<UnlockUserCommand>
{
    public UnlockUserCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID is required.");
    }
}