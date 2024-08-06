using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Customers.Data.Models;
using Customers.Admin.Services;
using Customers.Services.Contracts;
using Microsoft.AspNetCore.Identity;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class ChangePassword
    {
        [Inject]
        protected IJSRuntime JSRuntime { get; set; }

        [Inject]
        protected NavigationManager NavigationManager { get; set; }

        [Inject]
        protected DialogService DialogService { get; set; }

        [Inject]
        protected TooltipService TooltipService { get; set; }

        [Inject]
        protected ContextMenuService ContextMenuService { get; set; }
        [Inject]
        public IActivityLogService ActivityLogService { get; set; }
        [Inject]
        protected NotificationService NotificationService { get; set; }

        protected string oldPassword = "";
        protected string newPassword = "";
        protected string confirmPassword = "";
        protected ApplicationUser user;
        public ChangePasswordModel change = new ChangePasswordModel();
        protected string error;
        protected bool errorVisible;
        protected bool successVisible;
        protected string[] errorMessages;
        protected bool success = true;

        [Inject]
        protected IUserService UserService { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }

        bool isLoading = false;
        int id;

        protected override async Task OnInitializedAsync()
        {
           
            var claims = await Security.GetUserClaims();
            change.id = claims.UserId;
            user = await UserService.GetUserById(claims.UserId);
        }

        protected async Task FormSubmit()
        {
            try
            {
                isLoading = true;
                await UserService.ChangePassword(change);
                await ActivityLogService.AddActivityLog("User changed to new password");
                successVisible = true;
                success = false;
                isLoading = false;
            }
            catch (Exception ex)
            {

                isLoading = false;
                errorVisible = true;
                errorMessages = ex.Message.Split(',');
            }
        }
    }
}