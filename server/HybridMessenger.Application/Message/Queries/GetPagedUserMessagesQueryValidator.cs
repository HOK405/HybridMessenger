using FluentValidation;
using HybridMessenger.Application.Message.DTOs;
using System.Reflection;

namespace HybridMessenger.Application.Message.Queries
{
    public class GetPagedUserMessagesQueryValidator : AbstractValidator<GetPagedUserMessagesQuery>
    {
        public GetPagedUserMessagesQueryValidator()
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
