using DocumentFormat.OpenXml.InkML;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MySqlX.XDevAPI;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using System.Security.Claims;

namespace Customers.Admin.Services
{
    public class CustomClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly IUnitOfWork unitOfWork;        

        public CustomClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            IUnitOfWork unitOfWork)
            : base(userManager, roleManager, optionsAccessor)
        {
            this.unitOfWork = unitOfWork;            
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                var identity = await base.GenerateClaimsAsync(user);

                var customerId = await context.CustomerUsers
                    .Where(x => x.AspNetUserId == user.Id)
                    .Select(x => x.CustomerId).FirstOrDefaultAsync();
                var customer = await context.Customers.Where(x => x.CustomerId == customerId).FirstOrDefaultAsync();

                identity.AddClaim(new Claim("UserFullName", $"{user.FirstName} {user.LastName}"));
                identity.AddClaim(new Claim("UserId", user.Id.ToString()));

                if (customer is null)
                    return identity;

                identity.AddClaim(new Claim("CustomerName", customer.Name));
                identity.AddClaim(new Claim("CustomerId", customer.CustomerId.ToString()));
                //identity.AddClaim(new Claim("CustomerLogo", customer.logo));

                return identity;
            }
        }
    }
}
