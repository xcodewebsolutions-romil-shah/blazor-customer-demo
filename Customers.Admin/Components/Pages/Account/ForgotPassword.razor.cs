using DocumentFormat.OpenXml.Drawing;
using Microsoft.AspNetCore.Components;
using MySqlConnector.Logging;
using Radzen;
using Customers.Data.Models;
using Customers.Services.Contracts;
using Customers.Admin.Models;
using Customers.Admin.Services;
using System.Reflection;
using System.Security.Policy;
using Microsoft.AspNetCore.Identity;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class ForgotPassword
    {
        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        protected IUserService UserService { get; set; }
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        public IActivityLogService ActivityLogService { get; set; }
        protected AddUserModel user = new();
        Details Details = new Details();
        [Inject]
        public UserManager<ApplicationUser> userManager { get; set; }
        AddUserModel userModel { get; set; } = new();

        protected string error;
        protected bool errorVisible;
        bool SuccessVisible = false;
        bool ErrorVisible = false;
        bool forgotPassword = true;
        bool OTP = false;
        bool IsOTPcheckSuccess = false;
        bool changePassword = false;
        bool successVisible = false;
        string OTPSent;
        string IsUsercheckSuccess = null;
        protected string[] errorMessages;
        bool NotAllowed = false;

        protected async Task FormSubmit()
        {
            try
            {
                var IsExistingUser = user;
                Details.Email = user.Email;
                var checkuser = await userManager.FindByNameAsync(user.Email);
                if (!(await userManager.IsInRoleAsync(checkuser, "RMO Admin")))
                {
                    NotAllowed = true;
                    error = "You do not have permission to access this platform";
                    return;
                }

                IsUsercheckSuccess = await UserService.CkeckUser(IsExistingUser);
                if (IsUsercheckSuccess == "false")
                {
                    error = "Email not found";
                    await ActivityLogService.AddActivityLog($"An anonymous user attempted forgot password with email {user.Email}");
                    ErrorVisible = true;
                }
                else
                {
                    forgotPassword = false;
                    OTP = true;
                    SuccessVisible = true;
                    OTPSent = "An sms is sent to your registered mobile with one-time pin";
                    await ActivityLogService.AddActivityLog("Forgot Password is attempted, an OTP is sent to the email");
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = "Invalid User";
            }
        }

        protected async Task FormSubmitOTP()
        {
            try
            {
                IsOTPcheckSuccess = await UserService.CkeckOtp(Details);
                if (IsOTPcheckSuccess)
                {
                    OTP = false;
                    changePassword = true;
                    errorVisible = false;
                }
                else
                {
                    SuccessVisible = false;
                    errorVisible = true;
                    error = "Invalid OTP";
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }

        protected async Task FormSubmitChangePassword()
        {
            try
            {
                await UserService.ForgotPassword(Details);
                await ActivityLogService.AddActivityLog("User has set new password using Forgot Password");
                successVisible = true;
                errorVisible = false;
            }
            catch (Exception ex)
            {
                errorVisible = true;
                errorMessages = ex.Message.Split(',');
            }
        }
        protected void Success()
        {
            NavigationManager.NavigateTo("/login");
        }
    }

}
