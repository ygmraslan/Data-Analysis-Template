using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Users.Commands.UnlockUser;

public class UnlockUserCommand : IRequest<Result>
{
    public int Id { get; set; }
}