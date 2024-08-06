using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Repositories
{
    public class CustomerRepository : GenericRepository<Customer>, ICustomerRepository<Customer>
    {
        private readonly IDbContextFactory<CustomersDBContext> contextFactory;
        public CustomerRepository(IDbContextFactory<CustomersDBContext> contextFactory) : base(contextFactory)
        {
            this.contextFactory = contextFactory;
        }
        public async Task<Customer> GetCustomerAsync(int Id)
        {
            try
            {
                using (var context = contextFactory.CreateDbContext())
                {
                    var customerId = await context.CustomerUsers.Where(x => x.AspNetUserId == Id).Select(x => x.CustomerId).FirstOrDefaultAsync();
                    return await context.Customers.Where(x => x.CustomerId == customerId).FirstOrDefaultAsync();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
