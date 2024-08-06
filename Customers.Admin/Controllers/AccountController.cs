using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Customers.Admin.Models;
using Customers.Infrastructure.Helper;
using Customers.Data.Models;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using MySqlX.XDevAPI.Common;
using Microsoft.Extensions.Caching.Memory;
using System.Web;

namespace Customers.Admin.Controllers
{
    [Route("Account/[action]")]
    public partial class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<ApplicationRole> roleManager;
        private readonly IWebHostEnvironment env;
        private readonly IConfiguration configuration;
        private readonly OtpService _otpService;

        public AccountController(IWebHostEnvironment env, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager, IConfiguration configuration, OtpService otpService)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.env = env;
            this.configuration = configuration;
            _otpService = otpService;
        }

        private IActionResult RedirectWithError(string error, string redirectUrl = null)
        {
            if (!string.IsNullOrEmpty(redirectUrl))
            {
                return Redirect($"~/Login?error={error}&redirectUrl={Uri.EscapeDataString(redirectUrl.Replace("~", ""))}");
            }
            else
            {
                return Redirect($"~/Login?error={error}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl)
        {
            if (returnUrl != "/" && !string.IsNullOrEmpty(returnUrl))
            {
                return Redirect($"~/Login?redirectUrl={Uri.EscapeDataString(returnUrl)}");
            }

            return Redirect("~/Login");
        }

        [HttpPost]
        public async Task<IActionResult> Login(string userName, string password, string redirectUrl)
        {
            redirectUrl = string.IsNullOrEmpty(redirectUrl) ? "~/" : redirectUrl.StartsWith("/") ? redirectUrl : $"~/{redirectUrl}";

            if (env.EnvironmentName == "Development" && userName == "admin" && password == "admin")
            {
                var claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name, "admin"),
                    new Claim(ClaimTypes.Email, "admin")
                };

                roleManager.Roles.ToList().ForEach(r => claims.Add(new Claim(ClaimTypes.Role, r.Name)));
                await signInManager.SignInWithClaimsAsync(new ApplicationUser { Id = 1, SecurityStamp = "", UserName = userName, Email = userName }, isPersistent: false, claims);

                return Redirect(redirectUrl);
            }

            if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(password))
            {
                var user = await userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    return RedirectWithError("Invalid user or password", redirectUrl);
                }
                var checkPassword = await signInManager.CheckPasswordSignInAsync(user, password, true);
                if (!checkPassword.Succeeded)
                {
                    return RedirectWithError("Invalid user or password", redirectUrl);
                }

                if (user.TwoFactorEnabled)
                {
                    await signInManager.SignOutAsync();
                    var signInResult = await signInManager.PasswordSignInAsync(userName, password, false, false);
                    var token = await userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider);
                    Console.WriteLine(token);
                    if (!string.IsNullOrEmpty(token))
                    {
                        // var result = SMSHelper.SendMessage(token);
                        // return Redirect($"~/OTP");
                        _otpService.StoreOTP(token);
                        return Redirect($"~/verify-otp");

                    }
                }
                else
                {
                    await signInManager.SignInAsync(user, false);
                    return Redirect("/collections");
                }
            }

            return RedirectWithError("Invalid user or password", redirectUrl);
        }

        [HttpPost]
        public async Task<IActionResult> LoginWithOTP(string OTP)
        {
            var user = await signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return RedirectWithError("User not found.", $"~/Login?redirectUrl={Uri.EscapeDataString("/")}");
            }

            var signInResult = await signInManager.TwoFactorSignInAsync("Email", OTP, false, false);
            if (signInResult.Succeeded)
            {
                return Redirect("/collections");
            }
            else if (signInResult.IsLockedOut)
            {
                return RedirectWithError("User account locked out.", $"~/Login?redirectUrl={Uri.EscapeDataString("/")}");
            }
            else
            {
                return Redirect($"/verify-otp?error={Uri.EscapeDataString("Invalid OTP.")}");
            }
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            IdentityResult result;
            if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword))
            {
                return BadRequest("Invalid password");
            }

            var user = await userManager.FindByIdAsync(model.id.ToString());
            var isCurrentPasswordValid = await userManager.CheckPasswordAsync(user, model.OldPassword);
            if (!isCurrentPasswordValid)
            {
                return BadRequest("The current password is incorrect.");
            }
            else
            {
                result = await userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

                if (result.Succeeded)
                {
                    return Ok();
                }
            }

            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(message);
        }

        [HttpPost]
        public ApplicationAuthenticationState CurrentUser()
        {
            return new ApplicationAuthenticationState
            {
                IsAuthenticated = User.Identity.IsAuthenticated,
                Name = User.Identity.Name,
                Claims = User.Claims.Select(c => new ApplicationClaim { Type = c.Type, Value = c.Value })
            };
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();

            return Redirect("~/");
        }

        [Route("~/Account/SwitchRole/{userName}/{currentRole}/{currentUri}")]
        public async Task<IActionResult> SwitchRole(string userName, string currentRole, string currentUri)
        {
            try
            {
                // Decode the URI
                var decodedUriBytes = Convert.FromBase64String(currentUri);
                var decodedUri = Encoding.UTF8.GetString(decodedUriBytes);

                var user = await userManager.FindByNameAsync(userName);
                var claims = await userManager.GetClaimsAsync(user);
                var currentDbRole = claims.FirstOrDefault(x => x.Type == "Current Role");
                if (currentDbRole is not null)
                {
                    await userManager.RemoveClaimAsync(user, currentDbRole);
                }

                if (currentRole == "Account Owner")
                    await userManager.AddClaimAsync(user, new Claim("Current Role", "Account User"));
                else
                    await userManager.AddClaimAsync(user, new Claim("Current Role", "Account Owner"));

                await signInManager.SignOutAsync();
                await signInManager.SignInAsync(user, false);

                return Redirect(decodedUri != "blank" ? "/" + HttpUtility.UrlDecode(decodedUri) : "/");
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] AddUserModel userName)
        {
            if (!string.IsNullOrEmpty(userName.Email))
            {
                var user = await userManager.FindByNameAsync(userName.Email);
                if (user == null)
                {
                    return NotFound("User not found.");
                }
                else
                {
                    var token = await userManager.GeneratePasswordResetTokenAsync(user);


                    var otp = new Random().Next(100000, 999999).ToString();

                    Console.WriteLine(otp);

                    await _otpService.StoreOtpAsync(user.Id, otp, token);

                    // Send OTP to the user via email
                    //await SendOtpViaEmail(user.Email, otp);

                    return Ok(otp);
                }
            }
            return BadRequest("Email is required.");
        }

        [HttpPost]
        public async Task<IActionResult> ValidateOTP([FromBody] Details details)
        {

            var user = await userManager.FindByNameAsync(details.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var checkToken = await _otpService.RetrieveTokenAsync(user.Id, details.OTP);
            if (checkToken == null)
            {

                return BadRequest("Invalid OTP.");
            }
            else
            {
                return Ok();
            }
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody] Details details)
        {

            var user = await userManager.FindByNameAsync(details.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var storedToken = await _otpService.RetrieveTokenAsync(user.Id, details.OTP);
            if (storedToken == null)
            {
                return BadRequest("Invalid OTP.");
            }


            IdentityResult passwordChangeResult = await userManager.ResetPasswordAsync(user, storedToken, details.Password);
            if (passwordChangeResult.Succeeded)
            {
                // Optionally, delete the used OTP and token
                await _otpService.RemoveOtpAsync(user.Id, details.OTP);
                return Ok("Password reset successful.");
            }
            else
            {
                var message = string.Join(", ", passwordChangeResult.Errors.Select(error => error.Description));

                return BadRequest(message);
                //return BadRequest("Failed to reset password.");
            }


        }

    }

}
public class OtpService
{
    private readonly IMemoryCache _cache;
    private readonly TimeSpan _otpExpiration = TimeSpan.FromMinutes(10); // Example expiration time



    public OtpService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public void StoreOTP(string otp)
    {
        _cache.Set("OTP", otp);
    }

    public async Task StoreOtpAsync(int userId, string otp, string token)
    {
        var cacheKey = $"{userId}_{otp}";
        _cache.Set(cacheKey, token, _otpExpiration);
    }

    public async Task<string> RetrieveTokenAsync(int userId, string otp)
    {
        var cacheKey = $"{userId}_{otp}";
        _cache.TryGetValue(cacheKey, out string token);
        return token;
    }

    public async Task RemoveOtpAsync(int userId, string otp)
    {
        var cacheKey = $"{userId}_{otp}";
        _cache.Remove(cacheKey);
    }
}

