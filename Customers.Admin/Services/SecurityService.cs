using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

using Radzen;

using Customers.Admin.Models;
using Customers.Data.Models;
using System.Net;
using Customers.Admin.Components.Pages.Account;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System.ComponentModel.DataAnnotations;
using Customers.Data.Repositories;
using DocumentFormat.OpenXml.Spreadsheet;
using Customers.Infrastructure.Dtos;
using Microsoft.JSInterop;
using DocumentFormat.OpenXml.Wordprocessing;
using Customers.Data.Domains;
using Twilio.TwiML.Voice;

namespace Customers.Admin
{
    public partial class SecurityService(
        AuthenticationStateProvider authenticationStateProvider,
        NavigationManager navigationManager)
    {
        private ApplicationAuthenticationState AuthState { get; set; }
        private UserClaims UserClaims { get; set; }
        public async Task<ApplicationAuthenticationState> GetAuthenticationStateAsync()
        {
            if (AuthState != null)
                return AuthState;

            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            return AuthState = new()
            {
                IsAuthenticated = authState.User.Identity?.IsAuthenticated ?? false,
                Name = authState.User.Identity?.Name ?? "",
                Claims = authState.User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
            };
        }

        public async Task<UserClaims> GetUserClaims()
        {
            if (UserClaims != null)
                return UserClaims;

            var authState = await GetAuthenticationStateAsync();
            return UserClaims = new UserClaims()
            {
                UserId = Convert.ToInt32(authState.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "0"),
                UserFullName = authState.Claims.FirstOrDefault(x => x.Type == "UserFullName")?.Value ?? "",
                CurrentRole = authState.Claims.FirstOrDefault(x => x.Type == "Current Role")?.Value ?? "",
                CustomerId = Convert.ToInt32(authState.Claims.FirstOrDefault(x => x.Type == "CustomerId")?.Value ?? "0"),
                CustomerName = authState.Claims.FirstOrDefault(x => x.Type == "CustomerName")?.Value ?? "",
                CustomerLogo = authState.Claims.FirstOrDefault(x => x.Type == "CustomerLogo")?.Value ?? ""
            };
        }

        public async System.Threading.Tasks.Task Refresh()
        {
            var authState = await authenticationStateProvider.GetAuthenticationStateAsync();
            AuthState = new()
            {
                IsAuthenticated = authState.User.Identity?.IsAuthenticated ?? false,
                Name = authState.User.Identity?.Name ?? "",
                Claims = authState.User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
            };

            UserClaims = new UserClaims()
            {
                UserId = Convert.ToInt32(AuthState.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value ?? "0"),
                UserFullName = AuthState.Claims.FirstOrDefault(x => x.Type == "UserFullName")?.Value ?? "",
                CustomerId = Convert.ToInt32(AuthState.Claims.FirstOrDefault(x => x.Type == "CustomerId")?.Value ?? "0"),
                CurrentRole = AuthState.Claims.FirstOrDefault(x => x.Type == "Current Role")?.Value ?? "",
                CustomerName = AuthState.Claims.FirstOrDefault(x => x.Type == "CustomerName")?.Value ?? "",
                CustomerLogo = AuthState.Claims.FirstOrDefault(x => x.Type == "CustomerLogo")?.Value ?? ""
            };
        }


        public async Task<bool> SwitchRole(string currentRole, string userName, string currentUri)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            currentUri = currentUri == string.Empty ? "blank" : currentUri;            
            navigationManager.NavigateTo($"{navigationManager.BaseUri}Account/SwitchRole/{userName}/{currentRole}/{Convert.ToBase64String(Encoding.UTF8.GetBytes(currentUri))}", true);         

            return false;
        }

        public void Logout()
        {
            navigationManager.NavigateTo($"{navigationManager.BaseUri}Account/Logout", true);
        }
    }
}