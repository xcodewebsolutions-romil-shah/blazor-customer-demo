using Customers.Data.Domains;
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface ICollectionRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<Collection>> GetCollectionsNameAsync(string name, int id);
        Task<int> GetCustomerIDAsync(int id);
        Task<List<Collection>> GetCollectionAsync(int customerId, string collectionType);
        Task<List<Collection>> GetCollectionForUserAsync(int customerId, int userId);
        Task<List<Collection>> GetCollectionWithUsers();
        Task<KPIResultModel> GetKPIResult(int collectionId);
    }
}
