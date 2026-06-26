using FluentValidation;

namespace DataAnalysis.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommandValidator : AbstractValidator<ToggleUserStatusCommand>
{
    public ToggleUserStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("User ID is required.");
    }
}