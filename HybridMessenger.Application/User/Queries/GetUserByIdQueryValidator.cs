using FluentValidation;
using HybridMessenger.Domain.Services;

namespace HybridMessenger.Application.User.Queries
{
    public class GetUserByIdQueryValidator : AbstractValidator<GetUserByIdQuery>
    {
        private readonly IUserIdentityService _userIdentityService;
        public GetUserByIdQueryValidator(IUserIdentityService userIdentityService)
        {
            _userIdentityService = userIdentityService;

            RuleFor(query => query.Id)
            .NotEmpty().WithMessage("User ID is required.")
            .MustAsync(UserExistsAsync).WithMessage("No user found with the given ID.");
        }

        private async Task<bool> UserExistsAsync(int id, CancellationToken cancellationToken)
        {
            var user = await _userIdentityService.GetUserByIdAsync(id);
            return user != null;
        }
    }
}
