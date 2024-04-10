using FluentValidation;
using HybridMessenger.Application.User.DTOs;
using System.Reflection;

namespace HybridMessenger.Application.User.Queries
{
    public class GetPagedUsersQueryValidator : AbstractValidator<GetPagedUsersQuery>
    {
        public GetPagedUsersQueryValidator()
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
            return typeof(Domain.Entities.User).GetProperty(sortBy, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance) != null;
        }

        private bool BeValidRequestedFields(IEnumerable<string> fields)
        {
            var dtoProperties = typeof(UserDto).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                               .Select(p => p.Name)
                                               .ToList();

            return !fields.Except(dtoProperties, StringComparer.OrdinalIgnoreCase).Any();
        }
    }
}
