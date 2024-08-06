using Microsoft.AspNetCore.Components;
using Radzen;
using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Admin.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace Customers.Admin.Services
{
    public interface IUserService
    {
        // Task ChangePassword(string oldPassword, string newPassword, int id);
        Task ChangePassword(ChangePasswordModel change);
        Task<bool> CkeckOtp(Details details);
        Task<string> CkeckUser(AddUserModel user);
        Task<ApplicationRole> CreateRole(ApplicationRole role);
        Task<ApplicationUser> CreateUser(AddUserModel user);
        Task<HttpResponseMessage> DeleteRole(string id);
        Task<HttpResponseMessage> DeleteUser(string id, int ownerId);
        Task<bool> SetActiveStatus(int userId, int adminId);
        Task ForgotPassword(Details details);
        Task<string> GetOTP(AddUserModel add);
        Task<IEnumerable<ApplicationRole>> GetRoles();
        Task<ApplicationUser> GetUserById(int id);
        Task<IEnumerable<ApplicationUserViewModel>> GetUsers(int customerId);
        Task<ApplicationUser> UpdateUser(string id, ApplicationUser user);
        Task<List<ApplicationUser>> GetCustomerOwners(int customerId);
    }

    public class UserService(NavigationManager navigationManager, HttpClient httpClient) : IUserService
    {
        public async Task<IEnumerable<ApplicationRole>> GetRoles()
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationRoles");

            var response = await httpClient.GetAsync(uri);

            var result = await response.ReadAsync<IEnumerable<ApplicationRole>>();

            return result;
        }

        public async Task<ApplicationRole> CreateRole(ApplicationRole role)
        {

            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationRoles");

            var content = new StringContent(JsonSerializer.Serialize(role), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);
            var strContent = await response.Content.ReadAsStringAsync();
            return await response.ReadAsync<ApplicationRole>();
        }

        public async Task<HttpResponseMessage> DeleteRole(string id)
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationRoles/{id}");

            return await httpClient.DeleteAsync(uri);
        }

        public async Task<IEnumerable<ApplicationUserViewModel>> GetUsers(int customerId)
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationUsers/{customerId}");
            var response = await httpClient.GetAsync(uri);

            return await response.ReadAsync<IEnumerable<ApplicationUserViewModel>>();
        }

        public async Task<ApplicationUser> CreateUser(AddUserModel user)
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationUsers");

            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);
            return await response.ReadAsync<ApplicationUser>();
        }

        public async Task<HttpResponseMessage> DeleteUser(string id, int ownerId)
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationUsers/{id}/deleteby/{ownerId}");

            return await httpClient.DeleteAsync(uri);
        }

        public async Task<ApplicationUser> GetUserById(int id)
        {
            using (var client = new HttpClient())
            {
                var uri2 = new Uri($"{navigationManager.BaseUri}api/ApplicationUsers/GetApplicationUser/{id}");
                var response = await client.GetAsync(uri2);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                return await response.ReadAsync<ApplicationUser>();
            }
        }

        public async Task<ApplicationUser> UpdateUser(string id, ApplicationUser user)
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationUsers/{id}");

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Patch, uri)
            {
                Content = new StringContent(JsonSerializer.Serialize(new
                {
                    Password = user.Password,
                    Roles = user.Roles
                }), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(httpRequestMessage);
            var content = await response.Content.ReadAsStringAsync();
            return await response.ReadAsync<ApplicationUser>();
        }

        public async Task ChangePassword(ChangePasswordModel change)
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/ChangePassword");
            var content = new StringContent(JsonSerializer.Serialize(change), Encoding.UTF8, "application/json");
         
                var response = await httpClient.PostAsync(uri, content);

                if (!response.IsSuccessStatusCode)
                {
                    var message = response.Content.ReadAsStringAsync().Result;
                    throw new ApplicationException(message);
                }
        }


        public async Task<string> CkeckUser(AddUserModel user)
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/GetToken");

            var content = new StringContent(JsonSerializer.Serialize(user), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);
            var otp = await response.Content.ReadAsStringAsync();

            if ((response.StatusCode == HttpStatusCode.BadRequest) || (response.StatusCode == HttpStatusCode.NotFound))
            {
                return "false";

            }
            else
            {
                return otp;
            }
        }

        public async Task<bool> CkeckOtp(Details details)
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/ValidateOTP");

            var content = new StringContent(JsonSerializer.Serialize(details), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);

            if ((response.StatusCode == HttpStatusCode.BadRequest) || (response.StatusCode == HttpStatusCode.NotFound))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public async Task ForgotPassword(Details details)
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/ForgotPassword");

            var content = new StringContent(JsonSerializer.Serialize(details), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);


            if (!response.IsSuccessStatusCode)
            {
                var message = await response.Content.ReadAsStringAsync();

                throw new ApplicationException(message);
            }
        }

        public async Task<string> GetOTP(AddUserModel add)
        {
            var uri = new Uri($"{navigationManager.BaseUri}Account/Login");

            var content = new StringContent(JsonSerializer.Serialize(add), Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);
            var otp = await response.Content.ReadAsStringAsync();

            if ((response.StatusCode == HttpStatusCode.BadRequest) || (response.StatusCode == HttpStatusCode.NotFound))
            {
                return "false";
            }
            else
            {
                return otp;
            }
        }

        public async Task<List<ApplicationUser>> GetCustomerOwners(int customerId)
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationUsers/getCustomerOwners/{customerId}");

            var response = await httpClient.GetAsync(uri);
            return await response.ReadAsync<List<ApplicationUser>>();
        }

        public async Task<bool> SetActiveStatus(int userId, int adminId)
        {
            var uri = new Uri($"{navigationManager.BaseUri}api/ApplicationUsers/setActiveStatus/{userId}/admin/{adminId}");

            var response = await httpClient.GetAsync(uri);
            if (response.IsSuccessStatusCode)
                return true;

            return false;
        }
    }
}
