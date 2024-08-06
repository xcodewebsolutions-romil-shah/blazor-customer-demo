using Org.BouncyCastle.Asn1.Mozilla;
using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface ICustomerService
    {
        Task<List<AccountUsers>> GetAccountUsers(int customerId);
        Task<string> GetCustomerNameFromCollectionId(int collectionId);
    }
}
