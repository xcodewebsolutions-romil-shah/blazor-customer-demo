using Microsoft.EntityFrameworkCore;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Repositories
{
    public class CustomerLicenseRepository : GenericRepository<CustomerLicense>, ICustomerLicenseRepository<CustomerLicense>
    {
        public CustomerLicenseRepository(IDbContextFactory<CustomersDBContext> context) : base(context)
        {
        }
    }
}
