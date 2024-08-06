using Customers.Data.Domains;
using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    //public interface ILicenseService
    //{
    //    Task<LicenseDefinitionDto?> GetCustomerLicense(int customerId);
    //    Task<CustomerLicenseDto?> GetLicensenDefinition(int customerId);
    //}
    public interface ILicenseService
    {
        Task<IEnumerable<CustomerLicenseDto>?> GetCustomerLicense(int customerId);
        Task<IEnumerable<LicenseDefinitionDto>?> GetLicensenDefinition();
        Task<IEnumerable<CustomerLicenseDto>?> GetLicenseInActiveCustomer(int CustomerId);
    }
}
