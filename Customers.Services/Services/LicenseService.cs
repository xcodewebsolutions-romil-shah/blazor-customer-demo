using Customers.Data.Contracts;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using Customers.Data.Domains;
using Customers.Services.Contracts;
using Customers.Infrastructure.Dtos;
using Customers.Data.Repositories;

namespace Customers.Services.Services
{

    public class LicenseService(IUnitOfWork unitOfWork, IMapper _mapper) : ILicenseService
    {
        //public async Task<CustomerLicenseDto?> GetCustomerLicense(int customerId, bool isCurrent)
        //{
        //    var customerLicense = await unitOfWork.LicenseRepositary.GetCustomerLicenseAsync(customerId, isCurrent);
        //    return _mapper.Map<CustomerLicenseDto>(customerLicense);
        //}
        public async Task<IEnumerable<CustomerLicenseDto>?> GetCustomerLicense(int customerId)
        {
            var customerLicense = await unitOfWork.LicenseRepositary.GetCustomerLicenseAsync(customerId);
            return _mapper.Map<IList<CustomerLicense>, IList<CustomerLicenseDto>>(customerLicense.ToList());
        }

        public async Task<IEnumerable<CustomerLicenseDto>?> GetLicenseInActiveCustomer(int CustomerId)
        {
            var licenseInActiceCustomer = await unitOfWork.LicenseRepositary.GetLicenseInActiveCustomerAsync(CustomerId);
            return _mapper.Map<IList<CustomerLicense>, IList<CustomerLicenseDto>>(licenseInActiceCustomer.ToList());
        }

        public async Task<IEnumerable<LicenseDefinitionDto>?> GetLicensenDefinition()
        {
            var licenseDefinition = await unitOfWork.LicenseRepositary.GetLicensenDefinitionAsync();
          //  return _mapper.Map<LicenseDefinitionDto>(licenseDefinition);
            return _mapper.Map<IList<LicenseDefinition>, IList<LicenseDefinitionDto>>(licenseDefinition.ToList());
        }
    }
}
