using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
    public class CustomerUsersRepository : GenericRepository<CustomerUsers>, ICustomerUsersRepository<CustomerUsers>
    {
        public CustomerUsersRepository(IDbContextFactory<CustomersDBContext> context) : base(context)
        {   
        }
    }
}
