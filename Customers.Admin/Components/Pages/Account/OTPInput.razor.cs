using crypto;
using DocumentFormat.OpenXml.EMMA;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.JSInterop;
using Customers.Data.Models;
using Customers.Infrastructure.Dtos;
using Customers.Services.Contracts;
using Customers.Admin.Services;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class OTPInput
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public IMemoryCache Memory { get; set; }
        [Inject]
        public IActivityLogService ActivityLogService { get; set; }
        protected string redirectUrl;
        protected string error;
        protected string info;
        protected bool errorVisible;
        protected bool infoVisible;
        protected bool SuccessVisible = true;
        string OTP = "";
        [SupplyParameterFromForm]
        public OTPDto OTPModel { get; set; } = new();
        [Inject]
        public SignInManager<ApplicationUser> signInManager { get; set; }
        protected override async Task OnInitializedAsync()
        {
            var query = System.Web.HttpUtility.ParseQueryString(new Uri(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString()).Query);
            error = query.Get("error");

            info = query.Get("info");

            redirectUrl = query.Get("redirectUrl");

            errorVisible = !string.IsNullOrEmpty(error);
            if (errorVisible)
            {
                SuccessVisible = false;
            }

            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                errorVisible = true;
                error = "User not found.";
                NavigationManager.NavigateTo("Login");
                return;
            }

            infoVisible = !string.IsNullOrEmpty(info);
            Memory.TryGetValue("OTP", out string token);
            OTP = token;
            //OTPModel.OTP = token;
            StateHasChanged();
        }

        async Task SubmitOTP()
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                errorVisible = true;
                error = "User not found.";
                NavigationManager.NavigateTo("Login");
                return;                
            }

            var signInResult = await signInManager.TwoFactorSignInAsync("Email", OTPModel.OTP.ToString(), false, false);
            if (signInResult.Succeeded)
            {
                await ActivityLogService.AddActivityLog("Login OTP is verified");
                NavigationManager.NavigateTo("monitoring-logs");
                return;
            }
            else if (signInResult.IsLockedOut)
            {
                errorVisible = true;
                error = "User account locked out.";
                NavigationManager.NavigateTo($"Login?redirectUrl={Uri.EscapeDataString("/")}"); 
                return;
            }
            else
            {
                errorVisible = true;
                error = "Invalid OTP.";                
                await ActivityLogService.AddActivityLog("OTP verification failed");
                return;                
            }
        }
    }
}
