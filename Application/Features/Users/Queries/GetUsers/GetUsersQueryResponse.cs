using DataAnalysis.Application.Features.Users.Dtos;

namespace DataAnalysis.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryResponse
{
    public List<UserDto> Users { get; set; } = new();
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}