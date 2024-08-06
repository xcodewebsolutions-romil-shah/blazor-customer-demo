using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface IAnalysisReportParameterRepository <TEntity>: IGenericRepository<TEntity> where TEntity : class
    {
    }
}
