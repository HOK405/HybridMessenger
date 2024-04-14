using AutoMapper;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IUserIdentityService _userService;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserIdentityService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(Guid.Parse(request.Id));

            return _mapper.Map<UserDto>(user);
        }
    }
}