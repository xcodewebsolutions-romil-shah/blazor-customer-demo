using Customers.Data.Domains;
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface ICollectionAnalysisRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        Task<KPIResultModel> GetKPIResultForAnalysis(int collectionId, int analysisId);
    }
}
