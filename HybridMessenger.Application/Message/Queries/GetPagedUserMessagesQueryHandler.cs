using HybridMessenger.Application.Message.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;
using System.Reflection;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetPagedUserMessagesQueryHandler : IRequestHandler<GetPagedUserMessagesQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDynamicProjectionService _dynamicProjectionService;
        private readonly IMessageRepository _messageRepository;

        public GetPagedUserMessagesQueryHandler(IUnitOfWork unitOfWork, IDynamicProjectionService dynamicProjectionService = null)
        {
            _unitOfWork = unitOfWork;
            _dynamicProjectionService = dynamicProjectionService;
            _messageRepository = _unitOfWork.GetRepository<IMessageRepository>();
        }

        public async Task<IEnumerable<object>> Handle(GetPagedUserMessagesQuery request, CancellationToken cancellationToken)
        {
            Dictionary<string, object> filtersToSearch = new Dictionary<string, object>
            {
                { "UserID", Guid.Parse(request.UserId) }
            };     

            var query = await _messageRepository.GetPagedAsync(
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                sortBy: request.SortBy,
                filters: filtersToSearch,
                searchValue: request.SearchValue,
                ascending: request.Ascending);

            IEnumerable<string> fieldsToInclude = request.Fields != null && request.Fields.Any() ?
                request.Fields : typeof(MessageDto).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);

            var dynamicResults = _dynamicProjectionService.ProjectToDynamic<Domain.Entities.Message, MessageDto>(query.ToList(), fieldsToInclude);

            return dynamicResults;
        }
    }
}
