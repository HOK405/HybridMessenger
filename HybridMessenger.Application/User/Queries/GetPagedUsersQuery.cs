using HybridMessenger.Application.User.DTOs;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace HybridMessenger.Application.User.Queries
{
    public class GetPagedUsersQuery : IRequest<IQueryable<UserDto>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortBy { get; set; }
        public string SearchValue { get; set; }
        public bool Ascending { get; set; }
    }
}
