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
using Microsoft.AspNetCore.Components.Authorization;
using Customers.Admin.Services;
using Customers.Infrastructure.Helper;
using crypto;
using System.Security.Claims;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using DocumentFormat.OpenXml.InkML;
using System.Linq.Expressions;
    using Customers.Services.Services;
using Customers.Services.Contracts;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class ApplicationUsers
    {
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected IUserService UserService { get; set; }
        [Inject]
        public SecurityService Security { get; set; }
        [Inject]
        public NavigationManager NavManager { get; set; }
        [Inject]
        public IJSRuntime JsRuntime { get; set; }
        [Inject]
        public ErrorDialogService ErrorService { get; set; }
        [Inject]
        public IActivityLogService ActivityLogService { get; set; }
        [Inject]
        public NotificationService NotificationService { get; set; }
        protected IEnumerable<ApplicationUserViewModel> users;
        protected RadzenDataGrid<ApplicationUserViewModel> grid0;
        List<ApplicationUser> ApplicationOwners { get; set; }

        protected AddUserModel user;
        protected ApplicationRole role { get; set; }

        UserClaims Claims = new UserClaims();

        bool IsLoading { get; set; } = false;
        protected string error;
        protected bool errorVisible;
        DotNetObjectReference<ApplicationUsers> dotNetRef;
        [Inject]
        DateTimeHelper dateTimeHelper { get; set; }
        int? daysLeft;

        protected override async Task OnInitializedAsync()
        {
            Claims = await Security.GetUserClaims();
            await CheckRole();
            await GetApplicationOwners();
            await GetUsers();
            dotNetRef = DotNetObjectReference.Create(this);

        }

        async Task CheckRole()
        {
            try
            {
                if (Claims.CurrentRole != "Account Owner")
                    NavManager.NavigateTo("/");
            }
            catch (Exception ex)
            {
                await ErrorService.ShowError(ex);
            }
        }

        async Task GetUsers()
        {
            try
            {
                users = await UserService.GetUsers(Claims.CustomerId);
                var userList = users.ToList();
                var tasks = userList.Select(userList => dateTimeHelper.ConvertToLocalTimeZone(userList.CreatedOn)).ToArray();
                var results = await Task.WhenAll(tasks);
                for (int i = 0; i < users.Count(); i++)
                {
                    userList[i].CreatedOn = results[i].Value;
                }
                users = userList.AsEnumerable();
                StateHasChanged();
            }
            catch (Exception ex)
            {
                await ErrorService.ShowError(ex);
            }
        }

        async Task GetApplicationOwners()
        {
            ApplicationOwners = await UserService.GetCustomerOwners(Claims.CustomerId);
            var tasks = ApplicationOwners.Select(ApplicationOwners => dateTimeHelper.ConvertToLocalTimeZone(ApplicationOwners.CreatedOn)).ToArray();
            var results = await Task.WhenAll(tasks);
            for (int i = 0; i < ApplicationOwners.Count(); i++)
            {
                ApplicationOwners[i].CreatedOn = results[i].Value;
            }
            StateHasChanged();
        }

        void OnRowRender(RowRenderEventArgs<ApplicationUserViewModel> args)
        {
            try
            {
                if (IsDisabled(args.Data.Id))
                {
                    args.Attributes.Add("class", "disabled-row");

                    // Add a unique ID to the row element
                    var uniqueId = $"row-{args.Data.Id}";
                    args.Attributes.Add("id", uniqueId);
                }
            }
            catch (Exception ex)
            {
                ErrorService.ShowError(ex);
             
            }
        }

        bool IsDisabled(int userId)
        {
            try
            {
                var loggedInUser = ApplicationOwners.FirstOrDefault(x => x.Id == Claims.UserId);

                if (loggedInUser == null)
                    return true;

                if (loggedInUser.IsOwner && ApplicationOwners.Select(x => x.Id).Contains(userId))
                    return true;

                if (!loggedInUser.IsOwner && loggedInUser.Id == userId)
                    return true;

                return false;
            }
            catch (Exception ex)
            {
                ErrorService.ShowError(ex);
                return false;
            }
        }
        protected async Task AddClick()
        {
            try
            {
                await DialogService.OpenAsync<AddApplicationUser>("Add Account User", new Dictionary<string, object>
            {{ "ApplicationOwners", ApplicationOwners } });

                await GetUsers();
                await GetApplicationOwners();
            }
            catch (Exception ex)
            {
                await ErrorService.ShowError(ex);
            }
        }

        protected async Task RowSelect(ApplicationUserViewModel user)
        {
            //await DialogService.OpenAsync<EditApplicationUser>("Edit Application User",
            //    new Dictionary<string, object> { { "Id", user.Id } });

            //await GetUsers();
        }

        protected async Task DeleteClick(ApplicationUserViewModel user)
        {
            try
            {
                if (!IsLoading)
                {
                    if (IsDisabled(user.Id))
                        return;

                    var claims = await Security.GetUserClaims();
                    if (await DialogService.Confirm("Are you sure you want to delete this user?") == true)
                    {
                        await UserService.DeleteUser($"{user.Id}", claims.UserId);

                        await GetUsers();
                        await GetApplicationOwners();
                    }
                }
            }
            catch (Exception ex)
            {
                await ErrorService.ShowError(ex);
            }
        }
        protected async Task MeakeUSerSecondary(ApplicationUserViewModel user)
        {
            try
            {
                if (await DialogService.Confirm("Are you sure to make this user as Secondary Owner?", "Confirmation", new ConfirmOptions { OkButtonText = "Yes, I'm sure", CancelButtonText = "No" }) == true)
                {
                    var claims = await Security.GetUserClaims();
                    var addUser = new AddUserModel
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        IsOwner = true,
                        isSecondary = true,
                        CustomerId = claims.CustomerId,
                        CreatedById = claims.UserId
                    };
                    await UserService.CreateUser(addUser);
                    await ActivityLogService.AddActivityLog($"User {user.Email} is converted to secondary owner");
                    await GetUsers();
                    await GetApplicationOwners();
                }
            }
            catch (Exception ex)
            {
                await ErrorService.ShowError(ex);
            }
        }

        async Task ChangeActiveStatus(int userId)
        {
            try
            {
                if (!IsLoading)
                {
                    if (IsDisabled(userId))
                    {
                        await GetUsers();
                        return;
                    }

                    IsLoading = true;
                    TooltipService.Close();
                    var claims = await Security.GetUserClaims();
                    await UserService.SetActiveStatus(userId, claims.UserId);
                    IsLoading = false;
                }
                await GetUsers();
            }
            catch (Exception ex)
            {
                await ErrorService.ShowError(ex);
            }
        }
    }
}