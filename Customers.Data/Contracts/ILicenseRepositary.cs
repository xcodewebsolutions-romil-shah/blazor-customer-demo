using Customers.Data.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface ILicenseRepositary<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        Task<List<CustomerLicense>?> GetCustomerLicenseAsync(int customerId);
        Task<List<LicenseDefinition>?> GetLicensenDefinitionAsync();
        //Task<IEnumerable<CustomerLicense>?> GetLicenseInActiveCustomerAsync();
        Task<List<CustomerLicense>?> GetLicenseInActiveCustomerAsync(int CustomerId);
    }
}
