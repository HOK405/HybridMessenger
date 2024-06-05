using HybridMessenger.Domain.Services;
using MediatR;

namespace HybridMessenger.Application.User.Commands
{
    public class UploadAvatarCommandHandler : IRequestHandler<UploadAvatarCommand, string>
    {
        private readonly IUserIdentityService _userIdentityService;
        private readonly IBlobStorageService _blobStorageService;

        public UploadAvatarCommandHandler(IUserIdentityService userIdentityService, IBlobStorageService blobStorageService)
        {
            _userIdentityService = userIdentityService;
            _blobStorageService = blobStorageService;
        }

        public async Task<string> Handle(UploadAvatarCommand command, CancellationToken cancellationToken)
        {
            var user = await _userIdentityService.GetUserByIdAsync(command.UserId);

            if (user is null)
            {
                throw new ArgumentException("The provided user id is invalid.");
            }

            using (var stream = command.File.OpenReadStream())
            {
                var avatarUrl = await _blobStorageService.UploadFileAsync(stream, $"{command.UserId}_{command.File.FileName}");
                user.AvatarUrl = avatarUrl;

                var isSucceeded = await _userIdentityService.UpdateUser(user);

                if (isSucceeded)
                {
                    return avatarUrl;
                }
                else
                {
                    throw new ArgumentException("Update result doesn't indicate success.");
                }
            }
        }
    }
}
