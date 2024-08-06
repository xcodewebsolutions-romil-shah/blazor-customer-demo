using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Customers.Data.Contracts;
using Customers.Data.Models;
using Customers.Infrastructure.Dtos;
using Customers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Services
{
    public class CustomerService(IUnitOfWork unitOfWork,UserManager<ApplicationUser> userManager) : ICustomerService
    {
        public async Task<List<AccountUsers>> GetAccountUsers(int customerId)
        {
            var customerUsers = await unitOfWork.CustomerUsersRepository.Query(x=>x.CustomerId == customerId);
            var users = await userManager.Users.Where(u => !u.IsDeleted &&  customerUsers.Select(x => x.AspNetUserId).Contains(u.Id) ).ToListAsync();
            return users.Select(u => new AccountUsers()
            {
                Id = u.Id,
                Name = u.FirstName + " " + u.LastName,
            }).ToList();
        }

        public async Task<string> GetCustomerNameFromCollectionId(int collectionId)
        {
            var collection = await unitOfWork.CollectionRepository.QueryFirstOrDefaultAsync(s=>s.CollectionId == collectionId);

            if (collection == null) return string.Empty;

            var customer = await unitOfWork.CustomerRepository.QueryFirstOrDefaultAsync(c=>c.CustomerId == collection.CustomerId);
            return customer?.Name ?? string.Empty;
        }
    }
}
