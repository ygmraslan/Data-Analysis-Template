using DataAnalysis.Application.Common;
using MediatR;

namespace DataAnalysis.Application.Features.Users.Commands.ToggleUserStatus;

public class ToggleUserStatusCommand : IRequest<Result<bool>>
{
    public int Id { get; set; }
}