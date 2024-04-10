using AutoMapper;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;
using System.Reflection;

namespace HybridMessenger.Application.User.Queries
{
    public class GetPagedUsersQueryHandler : IRequestHandler<GetPagedUsersQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDynamicProjectionService _dynamicProjectionService;

        public GetPagedUsersQueryHandler(IUnitOfWork unitOfWork, IDynamicProjectionService dynamicProjectionService = null)
        {
            _unitOfWork = unitOfWork;
            _dynamicProjectionService = dynamicProjectionService;
        }

        public async Task<IEnumerable<object>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
        {
            var userRepository = _unitOfWork.GetRepository<IUserRepository>();

            var query = await userRepository.GetPagedAsync(request.PageNumber, request.PageSize, request.SortBy, null, request.SearchValue, request.Ascending);


            IEnumerable<string> fieldsToInclude = request.Fields != null && request.Fields.Any() ? 
                request.Fields : typeof(UserDto).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);

            var dynamicResults = _dynamicProjectionService.ProjectToDynamic<Domain.Entities.User, UserDto>(query.ToList(), fieldsToInclude);

            return dynamicResults;
        }
    }
}
