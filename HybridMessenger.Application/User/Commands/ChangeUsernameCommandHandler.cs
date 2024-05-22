using AutoMapper;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class ChangeUsernameCommandHandler : IRequestHandler<ChangeUsernameCommand, UserDto>
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IMapper _mapper;

        public ChangeUsernameCommandHandler(IUserIdentityService userIdentityService, IMapper mapper)
        {
            _userIdentityService = userIdentityService;
            _mapper = mapper;
        }

        public async Task<UserDto> Handle(ChangeUsernameCommand command, CancellationToken cancellationToken)
        {
            var user = await _userIdentityService.GetUserByIdAsync(command.UserId);

            if (user is null)
            {
                throw new ArgumentException("The provided user id is invalid.");
            }

            if (user.UserName != command.NewUsername)
            {
                user.UserName = command.NewUsername;
            }

            if (user.PhoneNumber != command.NewPhoneNumber)
            {
                user.PhoneNumber = command.NewPhoneNumber;
            }

            var isSucceeded = await _userIdentityService.UpdateUser(user);

            if (isSucceeded)
            {
                return _mapper.Map<UserDto>(user);
            }
            else
            {
                throw new ArgumentException("Update result doesn't indicate success.");
            }
        }
    }

}