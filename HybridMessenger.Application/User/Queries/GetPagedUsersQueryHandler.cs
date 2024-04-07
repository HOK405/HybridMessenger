using AutoMapper;
using AutoMapper.QueryableExtensions;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;
using System.Reflection;

namespace HybridMessenger.Application.User.Queries
{
    public class GetPagedUsersQueryHandler : IRequestHandler<GetPagedUsersQuery, IQueryable<UserDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPagedUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IQueryable<UserDto>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
        {
            var validSortProperty = typeof(Domain.Entities.User).GetProperty(request.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (validSortProperty == null)
            {
                throw new ArgumentException($"Invalid sort property: {request.SortBy}");
            }

            var userRepository = _unitOfWork.GetRepository<IUserRepository>();

            var query = await userRepository.GetPagedAsync(request.PageNumber, request.PageSize, request.SortBy, request.Ascending);

            var queryDto = query.ProjectTo<UserDto>(_mapper.ConfigurationProvider);

            return queryDto;
        }


        /*public async Task<IQueryable<Domain.Entities.User>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
        {
            var validSortProperty = typeof(Domain.Entities.User).GetProperty(request.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

            if (validSortProperty == null)
            {
                throw new ArgumentException($"Invalid sort property: {request.SortBy}");
            }

            var userRepostitory = _unitOfWork.GetRepository<IUserRepository>();

            var query = await userRepostitory.GetPagedAsync(request.PageNumber, request.PageSize, request.SortBy, request.Ascending);

            return query;
        }*/
    }
}
