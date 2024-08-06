using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using Customers.Admin.Models;

namespace Customers.Admin
{
    public class ApplicationAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly SecurityService securityService;
        private readonly NavigationManager navManager;
        private ApplicationAuthenticationState authenticationState;

        public ApplicationAuthenticationStateProvider(SecurityService securityService,NavigationManager NavManager)
        {
            this.securityService = securityService;
            navManager = NavManager;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var identity = new ClaimsIdentity();

            try
            {
                var state = await GetApplicationAuthenticationStateAsync();

                if (state.IsAuthenticated)
                {
                    identity = new ClaimsIdentity(state.Claims.Select(c => new Claim(c.Type, c.Value)), "Customers.User");
                }          
            }
            catch (HttpRequestException ex)
            {
            }

            var result = new AuthenticationState(new ClaimsPrincipal(identity));
            //await securityService.InitializeAsync(result);

            return result;
        }

        private async Task<ApplicationAuthenticationState> GetApplicationAuthenticationStateAsync()
        {
            if (authenticationState == null)
            {
                authenticationState = await securityService.GetAuthenticationStateAsync();
            }

            return authenticationState;
        }
    }
}