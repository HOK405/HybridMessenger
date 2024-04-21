using HybridMessenger.Application.Chat.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;
using System.Reflection;

namespace HybridMessenger.Application.Chat.Queries
{
    public class GetPagedUserChatsQueryHandler : IRequestHandler<GetPagedUserChatsQuery, IEnumerable<object>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDynamicProjectionService _dynamicProjectionService;
        private readonly IChatRepository _chatRepository;

        public GetPagedUserChatsQueryHandler(IUnitOfWork unitOfWork, IDynamicProjectionService dynamicProjectionService)
        {
            _unitOfWork = unitOfWork;
            _dynamicProjectionService = dynamicProjectionService;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();
        }

        public async Task<IEnumerable<object>> Handle(GetPagedUserChatsQuery request, CancellationToken cancellationToken)
        {

            var query = await _chatRepository.GetPagedUserChatsAsync(
                userId: request.UserId,
                pageNumber: request.PageNumber,
                pageSize: request.PageSize,
                sortBy: request.SortBy,
                searchValue: request.SearchValue,
                ascending: request.Ascending);

            IEnumerable<string> fieldsToInclude = request.Fields != null && request.Fields.Any() ?
                request.Fields : typeof(ChatDto).GetProperties(BindingFlags.Public | BindingFlags.Instance).Select(p => p.Name);

            var dynamicResults = _dynamicProjectionService.ProjectToDynamic<Domain.Entities.Chat, ChatDto>(query.ToList(), fieldsToInclude);

            return dynamicResults;
        }
    }
}
