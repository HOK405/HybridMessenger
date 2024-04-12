using AutoMapper;
using HybridMessenger.Application.Message.DTOs;

namespace HybridMessenger.Application.Shared.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Domain.Entities.Message, MessageDto>()
            .ForMember(dto => dto.MessageId, opt => opt.MapFrom(src => src.MessageID))
            .ForMember(dto => dto.ChatId, opt => opt.MapFrom(src => src.ChatID))
            .ForMember(dto => dto.UserId, opt => opt.MapFrom(src => src.UserID))
            .ForMember(dto => dto.MessageText, opt => opt.MapFrom(src => src.MessageText))
            .ForMember(dto => dto.SentAt, opt => opt.MapFrom(src => src.SentAt));
        }
    }
}
