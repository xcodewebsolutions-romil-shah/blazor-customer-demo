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
using Customers.Data;

namespace Customers.Data.Repositories
{
    public class LicenseRepositary : GenericRepository<LicenseDefinition>, ILicenseRepositary<LicenseDefinition>
    {
        private readonly IDbContextFactory<CustomersDBContext> contextFactory;
        public LicenseRepositary(IDbContextFactory<CustomersDBContext> contextFactory) : base(contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        //public async Task<CustomerLicense?> GetCustomerLicenseAsync(int customerId)
        //{
        //    CustomerLicense customerLicense;
        //    if (isCurrent)
        //    {
        //        customerLicense = await context.CustomerLicenses.FirstOrDefaultAsync(x => x.CustomerId == customerId && x.IsCurrent);
        //    }
        //    else
        //    {
        //        customerLicense = await context.CustomerLicenses.FirstOrDefaultAsync(x => x.CustomerId == customerId && !x.IsCurrent);
        //    }
        //    if (customerLicense == null)
        //    {
        //        return null;
        //    }
        //    return customerLicense;
        //}
        public async Task<List<CustomerLicense>?> GetCustomerLicenseAsync(int customerId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var customerLicense = await context.CustomerLicenses.Where(x => x.CustomerId == customerId).ToListAsync();

                return customerLicense;
            }
        }
        public async Task<List<LicenseDefinition>?> GetLicensenDefinitionAsync()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var licenseDefinition = await context.LicenseDefinitions.ToListAsync();
                if (licenseDefinition == null)
                {
                    return null;
                }
                return licenseDefinition;
            }
        }
        public async Task<List<CustomerLicense>?> GetLicenseInActiveCustomerAsync(int CustomerId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var customerLicense = await context.CustomerLicenses.Where(x => x.CustomerId == CustomerId && !x.IsCurrent).ToListAsync();
                return customerLicense;
            }
        }

    }
}

