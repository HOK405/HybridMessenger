using HybridMessenger.Application.User.DTOs;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetPagedUsersQuery : IRequest<IQueryable<UserDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public bool Ascending { get; set; }
    }
}
