using FluentValidation;
using HybridMessenger.Application.Chat.DTOs;
using System.Reflection;

namespace HybridMessenger.Application.Chat.Queries
{
    public class GetPagedUserChatsQueryValidator : AbstractValidator<GetPagedUserChatsQuery>
    {
        public GetPagedUserChatsQueryValidator()
        {
            RuleFor(query => query.SortBy)
                .Must(BeAValidSortProperty)
                .WithMessage("Invalid sort property.");

            RuleFor(query => query.Fields)
                .Must(BeValidRequestedFields)
                .When(query => query.Fields != null && query.Fields.Any())
                .WithMessage("Invalid field(s) requested.");
        }

        private bool BeAValidSortProperty(string sortBy)
        {
            return typeof(ChatDto).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        private bool BeValidRequestedFields(IEnumerable<string> fields)
        {
            var dtoProperties = typeof(ChatDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Select(p => p.Name)
                                               .ToList();

            return !fields.Except(dtoProperties, StringComparer.OrdinalIgnoreCase).Any();
        }
    }
}
