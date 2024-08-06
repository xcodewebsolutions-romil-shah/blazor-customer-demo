using Customers.Data.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface ICollectionSOWRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        Task<bool> SetCollectionSOWAsActive(int collectionSOWId, int collectionId);
        Task<bool> ArchiveCollection(int collectionSOWId);
        Task<bool> SetCollectionSOWAsNotActive(int collectionId);
    }
}
