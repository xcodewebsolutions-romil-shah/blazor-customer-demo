using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Radzen;
using Radzen.Blazor;
using Customers.Admin.Models;
using Customers.Data.Models;
using Customers.Admin.Services;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class EditApplicationUser
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
        protected NotificationService NotificationService { get; set; }

        protected IEnumerable<ApplicationRole> roles;
        protected ApplicationUser user;
        protected IEnumerable<int> userRoles;
        protected string error;
        protected bool errorVisible;

        [Parameter]
        public int Id { get; set; }

        [Inject]
        protected IUserService UserService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            user = await UserService.GetUserById(Convert.ToInt32(Id));
            if (user.Roles != null)
            {
                userRoles = user.Roles.Select(role => role.Id);
            }

            roles = await UserService.GetRoles();
        }

        protected async Task FormSubmit(ApplicationUser user)
        {
            try
            {
                var dbUser = await UserService.GetUserById(user.Id);
                dbUser.Roles = roles.Where(role => userRoles.Contains(role.Id)).ToList();
                dbUser.Email = user.Email;
                dbUser.UserName = user.UserName;
                dbUser.Password = user.Password;
                await UserService.UpdateUser($"{Id}", dbUser);
                DialogService.Close(null);
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }

        protected async Task CancelClick()
        {
            DialogService.Close(null);
        }
    }
}