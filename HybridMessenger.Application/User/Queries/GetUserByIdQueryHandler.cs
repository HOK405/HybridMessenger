﻿using AutoMapper;
using HybridMessenger.Application.User.DTOs;
using HybridMessenger.Domain.Services;
using HybridMessenger.Domain.UnitOfWork;
using MediatR;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, DTOs.UserDto>
    {
        private readonly IUserIdentityService _userService;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(IUserIdentityService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        public async Task<DTOs.UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userService.GetUserByIdAsync(request.Id);

            return _mapper.Map<UserDto>(user);
        }
    }
}