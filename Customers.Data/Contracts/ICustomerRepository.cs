using Customers.Data.Domains;
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface ICustomerRepository <TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        Task<Customer> GetCustomerAsync(int Id);
    }
}
