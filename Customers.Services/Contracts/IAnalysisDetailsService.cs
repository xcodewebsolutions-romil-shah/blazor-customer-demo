using Customers.Data.Domains;
using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface IAnalysisDetailsService
    {
        Task<IEnumerable<AnalysisDetailDto>> GetAnalysisDetailsAsync(int collection_analysis_id);
    }
}
