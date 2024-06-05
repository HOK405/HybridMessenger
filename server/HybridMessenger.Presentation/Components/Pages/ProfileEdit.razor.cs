using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.ResponseModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class ProfileEdit : ComponentBase
    {
        [Inject]
        private IHttpService HttpService { get; set; }

        [Inject]
        private AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private UserProfileResponse user = new UserProfileResponse();
        private UpdateProfileRequest profile = new UpdateProfileRequest();
        private int _userId;
        private bool isReadOnly = true;

        private IBrowserFile avatarFile;
        private string uploadedAvatarUrl;
        private string validationError;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _userId = int.Parse(authState.User.FindFirst("nameid")?.Value ?? "0");

            user = await HttpService.GetAsync<UserProfileResponse>($"api/user/get-by-id?Id={_userId}");

            profile.NewUsername = user.UserName;
            profile.NewPhoneNumber = user.PhoneNumber;
            uploadedAvatarUrl = user.AvatarUrl;
        }

        private void EnableEditing()
        {
            isReadOnly = false;
        }

        private async Task HandleAvatarUpload(InputFileChangeEventArgs e)
        {
            avatarFile = e.File;

            if (avatarFile != null)
            {
                var extension = Path.GetExtension(avatarFile.Name).ToLowerInvariant();
                if (extension != ".png" && extension != ".jpg" && extension != ".jpeg")
                {
                    validationError = "Only .png and .jpg files are allowed.";
                    avatarFile = null;
                }
                else
                {
                    validationError = null;
                }
            }
        }

        private async Task UploadAvatar()
        {
            if (avatarFile != null)
            {
                var content = new MultipartFormDataContent();
                using (var stream = avatarFile.OpenReadStream(10 * 1024 * 1024)) 
                {
                    content.Add(new StreamContent(stream), "file", avatarFile.Name);
                    var response = await HttpService.PostFileAsync<StringOkResponse>($"api/user/upload-avatar", content);
                    uploadedAvatarUrl = response.Message + "?" + DateTime.Now.Ticks; 
                    StateHasChanged(); 
                }
            }
        }

        private async Task HandleValidSubmit()
        {
            var result = await HttpService.PutAsync<UserProfileResponse>("api/user/update-profile", profile);
            isReadOnly = true;
        }
    }
}