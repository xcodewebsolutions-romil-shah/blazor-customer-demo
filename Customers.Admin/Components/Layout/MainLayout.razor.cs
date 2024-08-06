using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Customers.Admin.Components.Layout
{
    public partial class MainLayout
    {

        public bool sidebarExpanded { get; set; } = true;

        string? menuItem { get; set; } = "MONITORING LOGS";

        [CascadingParameter]
        private Task<AuthenticationState> authenticationStateTask { get; set; }
        public bool IsAccountOwner { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var authState = await authenticationStateTask;
            bool isAuthenticated = authState.User.Identity == null ? false : authState.User.Identity.IsAuthenticated;

            var claim = authState.User.FindFirstValue("Current Role") ?? string.Empty;
            IsAccountOwner = claim == "Account Owner";

            if (authState.User is null || !isAuthenticated)
                NavManager.NavigateTo("login");
        }
    }
}
