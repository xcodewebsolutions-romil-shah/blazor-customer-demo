using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Admin.Models;
using System.Security.Claims;
using Twilio.Jwt.AccessToken;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Customers.Infrastructure.Helper;
using DocumentFormat.OpenXml.Bibliography;
using MySqlX.XDevAPI.Common;
using Customers.Data;
using Customers.Services.Contracts;
using DocumentFormat.OpenXml.Spreadsheet;


namespace Customers.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUsersController : ControllerBase
    {
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly CustomersDBContext _dbContext;
        private readonly IActivityLogService _activityLogService;

        public ApplicationUsersController(IEmailService emailService,
            UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            CustomersDBContext customersDBContext,
            IActivityLogService activityLogService)
        {
            _emailService = emailService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _dbContext = customersDBContext;
            _activityLogService = activityLogService;
        }
        [HttpGet("{customerId}")]
        public async Task<IEnumerable<ApplicationUserViewModel>> Get(int customerId)
        {
            var customerUsers = await _unitOfWork.CustomerUsersRepository.Query(x => x.CustomerId == customerId);
            var users = await _userManager.Users.ToListAsync();
            var userForCustomer = users.Where(x => !x.IsDeleted && customerUsers.Select(cu => cu.AspNetUserId).Contains(x.Id));

            return userForCustomer.Select(u => new ApplicationUserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Email = u.Email,
                CreatedOn = u.CreatedOn,
                IsActive = u.IsActive,
                IsDeleted = u.IsDeleted,
                CreatedBy = (users.FirstOrDefault(x => x.Id == u.CreatedById)?.FirstName ?? "") + " "
                            + (users.FirstOrDefault(x => x.Id == u.CreatedById)?.LastName ?? "")
            });
        }

        [HttpGet("getCustomerOwners/{customerId}")]
        public async Task<IActionResult> GetCustomerOwner(int customerId)
        {
            var users = await _userManager.GetUsersInRoleAsync("Account Owner");
            var customerUsers = await _unitOfWork.CustomerUsersRepository.Query(x => x.CustomerId == customerId);

            var usersInCustomer = users.Where(x => customerUsers.Select(cu => cu.AspNetUserId).Contains(x.Id) &&
                                                                !x.IsDeleted).ToList();
            return Ok(usersInCustomer ?? new());
        }

        [HttpGet("setActiveStatus/{userId}/admin/{adminId}")]
        public async Task<IActionResult> setActiveStatus(int userId, int adminId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return NotFound();

                user.IsActive = !user.IsActive;
                user.ModifiedById = adminId;
                user.ModifiedOn = DateTime.UtcNow;

                var result = await _userManager.UpdateAsync(user);
                await _activityLogService.AddActivityLogForUser($"User {user.Email} is set {(!user.IsActive ? "inactive" : "active")}",adminId);
                return Ok(result.Succeeded);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("GetApplicationUser/{Id}")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(int Id)
        {
            var user = await _userManager.Users.Include(x => x.Roles).FirstOrDefaultAsync(x => x.Id == Id);
            return user;
        }

        [HttpDelete("{Id}/deleteby/{ownerId}")]
        public async Task<IActionResult> Delete(string Id, int ownerId)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }
            user.IsDeleted = true;
            user.DeletedOn = DateTime.UtcNow;
            user.DeletedById = ownerId;

            await _userManager.UpdateAsync(user);
            await _activityLogService.AddActivityLogForUser($"User {user.Email} is removed from the system", ownerId);
            //var result = await userManager.DeleteAsync(user);

            //if (!result.Succeeded)
            //{
            //    return IdentityError(result);
            //}

            return new NoContentResult();
        }

        [HttpPatch("{Id}")]
        public async Task<IActionResult> Patch(string Id, UpdateUserModel data)
        {
            var user = await _userManager.FindByIdAsync(Id);

            if (user == null)
            {
                return NotFound();
            }

            IdentityResult result = null;
            user.Roles = null;
            result = await _userManager.UpdateAsync(user);

            if (data.Roles != null)
            {
                result = await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

                if (result.Succeeded)
                {
                    result = await _userManager.AddToRolesAsync(user, data.Roles.Select(r => r.Name));
                }
            }

            if (!string.IsNullOrEmpty(data.Password))
            {
                result = await _userManager.RemovePasswordAsync(user);

                if (result.Succeeded)
                {
                    result = await _userManager.AddPasswordAsync(user, data.Password);
                }

                if (!result.Succeeded)
                {
                    return IdentityError(result);
                }
            }

            if (result != null && !result.Succeeded)
            {
                return IdentityError(result);
            }

            return new NoContentResult();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AddUserModel user)
        {
            IdentityResult result = null;
            ApplicationUser dbUser = _userManager.Users.FirstOrDefault(u => u.Email == user.Email);
            var existingUser = await _userManager.FindByNameAsync(user.Email);
            var generatedPassword = GeneratePassword();
            if (user.isSecondary)
            {
                if (dbUser != null)
                {
                    dbUser.IsOwner = user.IsOwner;
                    result = await _userManager.UpdateAsync(dbUser);

                    var dbUserRoles = await _userManager.GetRolesAsync(dbUser);
                    var dbUserClaims = await _userManager.GetClaimsAsync(dbUser);
                    var dbUserCustomer = await _unitOfWork.CustomerUsersRepository.Query(x => x.AspNetUserId == dbUser.Id);

                    if (dbUserCustomer != null)
                        await _unitOfWork.CustomerUsersRepository.DeleteRangeAsync(dbUserCustomer);

                    await _unitOfWork.CustomerUsersRepository.AddAsync(new CustomerUsers
                    {
                        AspNetUserId = dbUser.Id,
                        CustomerId = user.CustomerId
                    });

                    await _userManager.RemoveFromRolesAsync(dbUser, dbUserRoles);
                    await _userManager.RemoveClaimsAsync(dbUser, dbUserClaims);
                }

            }
            else
            {
                if (dbUser != null)
                {
                    dbUser.IsDeleted = false;
                    dbUser.FirstName = user.FirstName;
                    dbUser.LastName = user.LastName;
                    dbUser.IsOwner = user.IsOwner;
                    result = await _userManager.UpdateAsync(dbUser);

                    var token = await _userManager.GeneratePasswordResetTokenAsync(existingUser);
                    IdentityResult passwordChangeResult = await _userManager.ResetPasswordAsync(existingUser, token, generatedPassword);
                    await SendEmail(user, generatedPassword);

                    var dbUserRoles = await _userManager.GetRolesAsync(dbUser);
                    var dbUserClaims = await _userManager.GetClaimsAsync(dbUser);

                    var dbUserCustomer = await _unitOfWork.CustomerUsersRepository.Query(x => x.AspNetUserId == dbUser.Id);

                    if (dbUserCustomer != null)
                        await _unitOfWork.CustomerUsersRepository.DeleteRangeAsync(dbUserCustomer);

                    await _unitOfWork.CustomerUsersRepository.AddAsync(new CustomerUsers
                    {
                        AspNetUserId = dbUser.Id,
                        CustomerId = user.CustomerId
                    });

                    await _userManager.RemoveFromRolesAsync(dbUser, dbUserRoles);
                    await _userManager.RemoveClaimsAsync(dbUser, dbUserClaims);
                }
                else
                {
                    dbUser = new ApplicationUser
                    {
                        Email = user.Email,
                        UserName = user.Email,
                        EmailConfirmed = true,
                        TwoFactorEnabled = true,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        CreatedById = user.CreatedById,
                        CreatedOn = DateTime.UtcNow,
                        IsActive = true,
                        IsDeleted = false,
                        IsOwner = user.IsOwner,
                        DeletedOn = null,
                        DeletedById = null,
                        ModifiedById = null,
                        ModifiedOn = null
                    };
                    result = await _userManager.CreateAsync(dbUser, generatedPassword);

                    await SendEmail(user, generatedPassword);
                }

            }
            var role = user.IsOwner ? "Account Owner" : "Account User";
            if (result.Succeeded)
                result = await _userManager.AddToRoleAsync(dbUser, role);

            var customerUser = await _unitOfWork.CustomerUsersRepository
                .QueryFirstOrDefaultAsync(x => x.CustomerId == user.CustomerId && x.AspNetUserId == dbUser.Id);

            if (customerUser == null)
            {
                var isAdded = await _unitOfWork.CustomerUsersRepository.AddAsync(new CustomerUsers
                {
                    CustomerId = user.CustomerId,
                    AspNetUserId = dbUser.Id
                });

                if (isAdded.CustomerUseId > 0)
                    return Created($"odata/Identity/Users('{dbUser.Id}')", user);
                else
                    return IdentityError(result);
            }
            return Created($"odata/Identity/Users('{dbUser.Id}')", user);

        }
        public async Task SendEmail(AddUserModel user, string generatedPassword)
        {
            if (!string.IsNullOrEmpty(generatedPassword))
            {
                try
                {
                    var isEmailSent = await Email(generatedPassword, user);

                    if (isEmailSent)
                    {
                        Console.WriteLine("Email Sent Succcessfully");
                    }
                    else
                    {
                        Console.WriteLine("Email does not sent Succcessfully");
                    }
                }
                catch (Exception Ex)
                {
                    Console.WriteLine(Ex.Message);
                }
            }
        }
        private IActionResult IdentityError(IdentityResult result)
        {
            var message = string.Join(", ", result.Errors.Select(error => error.Description));

            return BadRequest(new { error = new { message } });
        }
        public async Task<bool> Email(string generatedPassword, AddUserModel user)
        {
            var firstName = _userManager.Users.Where(x => x.Id == user.CreatedById).Select(x => x.FirstName).FirstOrDefault();
            var lastName = _userManager.Users.Where(x => x.Id == user.CreatedById).Select(x => x.LastName).FirstOrDefault();
            var url = _configuration.GetValue<string>("URL");
            EmailRequest emailRequest = new EmailRequest();
            emailRequest.ToEmail = user.Email;//"yogesh@xcodewebsolutions.com"
            emailRequest.Subject = " Welcome";
            emailRequest.Body = $"<p>Hi {user.FirstName},</p>" +
                  $"<p>You have been invited by {firstName} {lastName} to join RMO. Please login with the below details:</p>" +
                  $"<p>RMO Url: <a href='{url}'>{url}</a></p>" +
                  $"<p>Username: {user.Email}</p>" +
                  $"<p>Password: {generatedPassword}</p>";
            var result = await _emailService.SendEmailAsync(emailRequest.ToEmail, emailRequest.Subject, emailRequest.Body, emailRequest.CcEmails);
            var logs = new LogEntry();
            logs.Message = "In Email Method";
            logs.Exception = "No Exception";
            _dbContext.LogEntries.Add(logs);
            _dbContext.SaveChanges();
            return result;
        }
        public class EmailRequest
        {
            public string ToEmail { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public List<string> CcEmails { get; set; } = new List<string>();
        }
        public static string GeneratePassword()
        {
            var options = new PasswordOptions
            {
                RequiredLength = 8,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireNonAlphanumeric = true,
                RequiredUniqueChars = 4
            };

            var random = new Random();
            var passwordChars = new System.Collections.Generic.List<char>();

            if (options.RequireDigit)
            {
                passwordChars.Add(GetRandomChar("0123456789", random));
            }

            if (options.RequireLowercase)
            {
                passwordChars.Add(GetRandomChar("abcdefghijklmnopqrstuvwxyz", random));
            }

            if (options.RequireUppercase)
            {
                passwordChars.Add(GetRandomChar("ABCDEFGHIJKLMNOPQRSTUVWXYZ", random));
            }

            if (options.RequireNonAlphanumeric)
            {
                passwordChars.Add(GetRandomChar("!@#$%^&*()-_=+[]{}|;:'\",.<>/?", random));
            }

            while (passwordChars.Count < options.RequiredLength)
            {
                passwordChars.Add(GetRandomChar("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()-_=+[]{}|;:'\",.<>/?", random));
            }

            while (passwordChars.Distinct().Count() < options.RequiredUniqueChars)
            {
                passwordChars[random.Next(passwordChars.Count)] = GetRandomChar("0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!@#$%^&*()-_=+[]{}|;:'\",.<>/?", random);
            }

            return new string(passwordChars.OrderBy(c => random.Next()).ToArray());

        }
        private static char GetRandomChar(string charSet, Random random)
        {
            return charSet[random.Next(charSet.Length)];
        }

    }
}

