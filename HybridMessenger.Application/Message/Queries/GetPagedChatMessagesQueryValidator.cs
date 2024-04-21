using FluentValidation;
using HybridMessenger.Application.Message.DTOs;
using HybridMessenger.Domain.Repositories;
using HybridMessenger.Domain.UnitOfWork;
using System.Reflection;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetPagedChatMessagesQueryValidator : AbstractValidator<GetPagedChatMessagesQuery>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IChatRepository _chatRepository;

        public GetPagedChatMessagesQueryValidator(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _chatRepository = _unitOfWork.GetRepository<IChatRepository>();

            RuleFor(query => query.ChatId)
                .NotEmpty().WithMessage("Chat ID cannot be empty")
                .MustAsync(async (chatId, cancellation) => await ChatExists(chatId)).WithMessage("Invalid chat id.");

            RuleFor(query => query.SortBy)
                    .Must(BeAValidSortProperty)
                    .WithMessage("Invalid sort property.");

            RuleFor(query => query.Fields)
                    .Must(BeValidRequestedFields)
                    .When(query => query.Fields != null && query.Fields.Any())
                    .WithMessage("Invalid field(s) requested.");
        }

        private async Task<bool> ChatExists(int chatId)
        {
            return await _chatRepository.ExistsAsync(chatId);
        }

        private bool BeAValidSortProperty(string sortBy)
        {
            return typeof(MessageDto).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        private bool BeValidRequestedFields(IEnumerable<string> fields)
        {
            var dtoProperties = typeof(MessageDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Select(p => p.Name)
                                               .ToList();

            return !fields.Except(dtoProperties, StringComparer.OrdinalIgnoreCase).Any();
        }
    }
}
