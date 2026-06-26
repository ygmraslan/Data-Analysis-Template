using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Users.Commands.ResetUserMfa;

public class ResetUserMfaCommand : IRequest<Result>
{
    public int Id { get; set; }
}