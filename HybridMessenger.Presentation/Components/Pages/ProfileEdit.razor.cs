using HybridMessenger.Presentation.RequestModels;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class ProfileEdit : ComponentBase
    {
        [Inject]
        private IHttpService HttpService { get; set; }

        [Inject]
        AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        private UserResponse user;
        private UpdateProfileRequest profile = new UpdateProfileRequest();
        private int _userId;
        private bool isReadOnly = true;

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            _userId = int.Parse(authState.User.FindFirst("nameid")?.Value ?? "0");

            user = await HttpService.GetAsync<UserResponse>($"api/user/get-by-id?Id={_userId}");

            profile.NewUsername = user.UserName;
            profile.NewPhoneNumber = user.PhoneNumber;
        }

        private void EnableEditing()
        {
            isReadOnly = false;
        }

        private async Task HandleValidSubmit()
        {
            var result = await HttpService.PutAsync<UserResponse>("api/user/update-profile", profile);
            isReadOnly = true; 
        }
    }
}