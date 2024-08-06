using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Microsoft.AspNetCore.Identity;
using Customers.Data.Models;
using Customers.Infrastructure.Dtos;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using MySqlX.XDevAPI;
using Customers.Services.Contracts;
using System.Runtime.InteropServices;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class Login
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }
        [Inject]
        protected NavigationManager NavigationManager { get; set; }
        [Inject]
        public IdentityRedirectManager IdentityRedirectManager { get; set; }
        [Inject]
        public SignInManager<ApplicationUser> signInManager { get; set; }
        [Inject]
        public UserManager<ApplicationUser> userManager { get; set; }
        [Inject]
        IWebHostEnvironment env { get; set; }
        [Inject]
        public RoleManager<ApplicationRole> roleManager { get; set; }
        [Inject]
        public OtpService _otpService { get; set; }
        [Inject]
        public IActivityLogService ActivityLogService { get; set; }

        private EditContext? editContext;
        protected string redirectUrl;
        protected string error;
        protected string info;
        protected bool errorVisible;
        protected bool infoVisible;
        [SupplyParameterFromForm]
        UserLoginDto User { get; set; } = new();

        private ValidationMessageStore? messageStore;


        protected override async Task OnInitializedAsync()
        {
            var query = System.Web.HttpUtility.ParseQueryString(new Uri(NavigationManager.ToAbsoluteUri(NavigationManager.Uri).ToString()).Query);

            error = query.Get("error");

            info = query.Get("info");

            redirectUrl = query.Get("redirectUrl");

            errorVisible = !string.IsNullOrEmpty(error);

            infoVisible = !string.IsNullOrEmpty(info);

            User ??= new();
            editContext = new(User);
            editContext.OnValidationRequested += HandleValidationRequested;
            messageStore = new(editContext);
        }

        private void HandleValidationRequested(object? sender,
        ValidationRequestedEventArgs args)
        {
            messageStore?.Clear();

            // Custom validation logic
            if (String.IsNullOrEmpty(User.EmailAddress))
            {
                messageStore?.Add(() => User.EmailAddress, "");
            }
            if (String.IsNullOrEmpty(User.Password))
            {
                messageStore?.Add(() => User.Password, "");
            }
        }


        async Task SubmitLogin()
        {
            redirectUrl = string.IsNullOrEmpty(redirectUrl) ? "~/" : redirectUrl.StartsWith("/") ? redirectUrl : $"~/{redirectUrl}";

            if (env.EnvironmentName == "Development" && User.EmailAddress == "admin" && User.Password == "admin")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin")
                };

                roleManager.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
                await signInManager.SignInWithClaimsAsync(new ApplicationUser { Id = 1, SecurityStamp = "", UserName = User.EmailAddress, Email = User.EmailAddress }, isPersistent: false, claims);

                IdentityRedirectManager.RedirectTo(redirectUrl);
                return;
            }

            if (!string.IsNullOrEmpty(User.EmailAddress) && !string.IsNullOrEmpty(User.Password))
            {
                var user = await userManager.FindByNameAsync(User.EmailAddress);
                if (user == null)
                {
                    errorVisible = true;
                    error = "Invalid user or password";
                    await ActivityLogService.AddActivityLog($"An anonymous user attempted to login with email {User.EmailAddress}");
                    return;
                    //RedirectWithError("", redirectUrl);
                }
               
                if (user.IsDeleted)
                {
                    errorVisible = true;
                    error = "You no longer have access to the platform. Please contact your administrator";
                    return;
                }
                if (!user.IsActive)
                {
                    errorVisible = true;
                    error = "You are no longer an active user";
                    return;
                }
                if (!(await userManager.IsInRoleAsync(user, "RMO Admin")))
                {
                    errorVisible = true;
                    error = "You do not have permission to access this platform";
                    return;
                }

                var checkPassword = await signInManager.CheckPasswordSignInAsync(user, User.Password, true);
                if (!checkPassword.Succeeded)
                {
                    errorVisible = true;
                    error = "Invalid user or password";
                    User.Password = null;
                    await ActivityLogService.AddActivityLog("Attempted to login with wrong password");
                    return;
                    //RedirectWithError("Invalid user or password", redirectUrl);
                }

                //var roles = await userManager.GetRolesAsync(user);
                //var claims = await userManager.GetClaimsAsync(user);
                //if (roles.Contains("Account Owner"))
                //{
                //    var currentRole = claims.FirstOrDefault(x => x.Type == "Current Role");
                //    if (currentRole != null && currentRole.Value == "Account User")
                //    {
                //        await userManager.RemoveClaimAsync(user, currentRole);
                //    }
                //    if (currentRole is null || currentRole.Value != "Account Owner")
                //    {
                //        await userManager.AddClaimAsync(user, new Claim("Current Role", "Account Owner"));
                //    }
                //}
                //if (roles.Contains("Account User") && !roles.Contains("Account Owner"))
                //{
                //    var currentRole = claims.FirstOrDefault(x => x.Type == "Current Role");
                //    if (currentRole == null || currentRole.Value != "Account User")
                //    {
                //        await userManager.AddClaimAsync(user, new Claim("Current Role", "Account User"));
                //    }
                //}

                if (user.TwoFactorEnabled)
                {
                    await signInManager.SignOutAsync();
                    var signInResult = await signInManager.PasswordSignInAsync(User.EmailAddress, User.Password, false, false);
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                    Console.WriteLine(token);
                    if (!string.IsNullOrEmpty(token))
                    {
                        // var result = SMSHelper.SendMessage(token);
                        // return Redirect($"~/OTP");
                        _otpService.StoreOTP(token);
                        await ActivityLogService.AddActivityLog("Attempted to login with valid credentials");
                        NavigationManager.NavigateTo("verify-otp");
                        return;
                        //Redirect($"~/verify-otp");

                    }
                }
                else
                {
                    await signInManager.SignInAsync(user, false);
                    IdentityRedirectManager.RedirectTo("monitoring-logs");
                    return;
                    //Redirect("/collections");
                }
            }
            errorVisible = true;
            error = "Invalid user or password";
            return;
            //RedirectWithError("Invalid user or password", redirectUrl);
        }
    }
}