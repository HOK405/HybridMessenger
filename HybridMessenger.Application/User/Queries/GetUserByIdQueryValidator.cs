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
            .MustAsync(AsyncGuidExists).WithMessage("No user found with the given ID.");
        }

        private async Task<bool> AsyncGuidExists(string guidString, CancellationToken cancellationToken)
        {
            if (!Guid.TryParse(guidString, out Guid parsedGuid))
            {
                return false;
            }

            var user = await _userIdentityService.GetUserByIdAsync(parsedGuid);
            return user != null;
        }
    }
}
