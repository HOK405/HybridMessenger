using Blazorise.DataGrid;
using HybridMessenger.Presentation.Models;
using HybridMessenger.Presentation.Services;
using Microsoft.AspNetCore.Components;

namespace HybridMessenger.Presentation.Components.Pages
{
    public partial class PagedUsers : ComponentBase
    {
        [Inject]
        private IHttpService HttpService { get; set; }

        private List<UserResponse> users = new List<UserResponse>();
        private int totalUsers;
        private UserSortParametersModel sortModel;

        protected override async Task OnInitializedAsync()
        {
            sortModel = new UserSortParametersModel() { PageNumber = 1, PageSize = 5, SortBy = "Id", Ascending = true};
            await LoadUsers();
        }

        private async Task LoadUsers()
        {
            var response = await HttpService.PostAsync<IEnumerable<UserResponse>>("api/User/get-paged-users", sortModel);

            users = response?.ToList() ?? new List<UserResponse>();
            totalUsers = users.Count;
            StateHasChanged();
        }

        public class UserResponse
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public DateTime CreatedAt { get; set; }
            public string PhoneNumber { get; set; }
        }
    }
}
