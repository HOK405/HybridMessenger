using HybridMessenger.Application.Chat.DTOs;
using AutoMapper;

namespace HybridMessenger.Application.Shared.Profiles
{
    public class ChatProfile : Profile
    {
        public ChatProfile()
        {
            CreateMap<Domain.Entities.Chat, ChatDto>()
             .ForMember(dto => dto.ChatId, opt => opt.MapFrom(src => src.ChatID))
             .ForMember(dto => dto.ChatName, opt => opt.MapFrom(src => src.ChatName))
             .ForMember(dto => dto.IsGroup, opt => opt.MapFrom(src => src.IsGroup))
             .ForMember(dto => dto.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));
        }     
    }
}
