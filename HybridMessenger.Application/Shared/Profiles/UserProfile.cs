using HybridMessenger.Application.User.DTOs;
using AutoMapper;

namespace HybridMessenger.Application.Shared.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<Domain.Entities.User, UserDto>()
                .ForMember(dto => dto.Id, opt => opt.MapFrom(user => user.Id))
                .ForMember(dto => dto.UserName, opt => opt.MapFrom(user => user.UserName))
                .ForMember(dto => dto.Email, opt => opt.MapFrom(user => user.Email))
                .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(user => user.CreatedAt))
                .ForMember(dto => dto.PhoneNumber, opt => opt.MapFrom(user => user.PhoneNumber));
        }
    }
}
