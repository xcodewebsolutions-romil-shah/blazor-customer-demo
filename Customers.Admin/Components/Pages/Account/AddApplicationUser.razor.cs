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
using Microsoft.AspNetCore.Identity;
using Customers.Data.Models;
using Customers.Admin.Services;
using crypto;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using Customers.Services.Contracts;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Customers.Admin.Components.Pages.Account
{
    public partial class AddApplicationUser
    {        
        [Inject]
        protected DialogService DialogService { get; set; }
        [Inject]
        protected SecurityService Security { get; set; }
        [Inject]
        public UserManager<ApplicationUser> userManager { get; set; }
        [Inject]
        public IActivityLogService ActivityLogService { get; set; }
        [Inject]
        protected IUserService UserService { get; set; }
        [Parameter]
        public List<ApplicationUser> ApplicationOwners { get; set; }

        protected IEnumerable<ApplicationRole> roles;
        protected AddUserModel user;
        protected IEnumerable<int> userRoles = Enumerable.Empty<int>();
        protected string error;
        protected bool errorVisible;
        string DomainErrorMessage { get; set; }
        bool isLoading = false;
        protected override async Task OnInitializedAsync()
        {
            user = new AddUserModel();
        }

        protected async Task FormSubmit(AddUserModel user)
        {
            try
            {
                isLoading = true;                
                var claims = await Security.GetUserClaims();
                var addUser = new AddUserModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    IsOwner = user.IsOwner,
                    CustomerId = claims.CustomerId,
                    CreatedById = claims.UserId
                };

                await UserService.CreateUser(addUser);
                var role = user.IsOwner ? "Secondary Owner" : "Account User";
                await ActivityLogService.AddActivityLog($"New user {user.Email} is added to the system as {role}");
                DialogService.Close(null);
                isLoading = false;
            }
            catch (Exception ex)
            {
                isLoading = false;
                errorVisible = true;
                error = ex.Message;
            }
        }

        protected void CancelClick()
        {
            DialogService.Close(null);
        }

        bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return true;

            var user = userManager.Users.FirstOrDefault(x=>x.Email == email && !x.IsDeleted);
            if (user != null)
            {
                ActivityLogService.AddActivityLog($"Attempted to add a duplicate user with email {email}");
                return false;
            }

            return true;
        }

        bool ValidateDomain(string email)
        {
            if (string.IsNullOrEmpty(email))
                return true;

            if (!new EmailAddressAttribute().IsValid(email))
                return true;

            var owner = ApplicationOwners.FirstOrDefault(x => !x.IsOwner);
            var ownerDomain = new MailAddress(owner?.Email ?? "").Host;


            var userDomain = new MailAddress(email ?? "").Host;

            if (ownerDomain != userDomain)
            {
                DomainErrorMessage = $"Email domain must be {ownerDomain}";
                return false;
            }

            return true;
        }
    }
}