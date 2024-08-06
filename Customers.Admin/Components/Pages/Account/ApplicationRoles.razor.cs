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
using crypto;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class ApplicationRoles
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
        protected RadzenDataGrid<ApplicationRole> grid0;
        protected string error;
        protected bool errorVisible;

        [Inject]
        protected IUserService UserService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            roles = await UserService.GetRoles();
        }

        protected async Task AddClick()
        {
            await DialogService.OpenAsync<AddApplicationRole>("Add Application Role");

            roles = await UserService.GetRoles();
        }

        protected async Task DeleteClick(ApplicationRole role)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure you want to delete this role?") == true)
                {
                    await UserService.DeleteRole($"{role.Id}");

                    roles = await UserService.GetRoles();
                }
            }
            catch (Exception ex)
            {
                errorVisible = true;
                error = ex.Message;
            }
        }
    }
}