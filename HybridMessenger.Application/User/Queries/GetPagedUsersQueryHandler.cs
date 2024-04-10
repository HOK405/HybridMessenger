using AutoMapper;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;
using System.Dynamic;
using System.Reflection;

namespace HybridMessenger.Application.User.Queries
{
    public class GetPagedUsersQueryHandler : IRequestHandler<GetPagedUsersQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetPagedUsersQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<IEnumerable<object>> Handle(GetPagedUsersQuery request, CancellationToken cancellationToken)
        {
            // SortFields validation
            var validSortProperty = typeof(Domain.Entities.User).GetProperty(request.SortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (validSortProperty == null)
            {
                throw new ArgumentException($"Invalid sort property: {request.SortBy}");
            }

            var userRepository = _unitOfWork.GetRepository<IUserRepository>();
            // SortWithPagination + Filtering
            var query = await userRepository.GetPagedAsync(request.PageNumber, request.PageSize, request.SortBy, request.SearchValue, request.Ascending);

            var dtoProperties = typeof(UserDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                .Select(p => p.Name)
                                                .ToList();

            if (request.Fields != null && request.Fields.Any(field => !dtoProperties.Contains(field, StringComparer.OrdinalIgnoreCase)))
            {
                var invalidFields = request.Fields.Where(field => !dtoProperties.Contains(field, StringComparer.OrdinalIgnoreCase))
                                                  .ToList();
                throw new ArgumentException($"Invalid field(s) requested: {string.Join(", ", invalidFields)}");
            }

            IEnumerable<string> fieldsToInclude = request.Fields != null && request.Fields.Any() ?
                                                  request.Fields.Intersect(dtoProperties, StringComparer.OrdinalIgnoreCase) :
                                                  dtoProperties;

            var dynamicResults = query.ToList().Select(user =>
            {
                IDictionary<string, object> expando = new ExpandoObject();

                foreach (var fieldName in fieldsToInclude)
                {
                    var propertyInfo = typeof(UserDto).GetProperty(fieldName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                    if (propertyInfo != null)
                    {
                        var value = propertyInfo.GetValue(_mapper.Map<UserDto>(user), null);
                        expando.Add(fieldName, value);
                    }
                }

                return (object)expando;
            });

            return dynamicResults;
        }

    }
}
