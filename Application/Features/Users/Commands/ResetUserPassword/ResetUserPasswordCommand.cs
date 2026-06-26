using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Users.Commands.ResetUserPassword;

public class ResetUserPasswordCommand : IRequest<Result>
{
    public int Id { get; set; }
}