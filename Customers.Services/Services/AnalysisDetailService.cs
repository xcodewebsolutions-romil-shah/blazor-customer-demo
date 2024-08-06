using AutoMapper;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Infrastructure.Dtos;
using Customers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Services
{
    public class AnalysisDetailsService(IUnitOfWork unitOfWork,IMapper map) : IAnalysisDetailsService
    {        
        public async Task<IEnumerable<AnalysisDetailDto>> GetAnalysisDetailsAsync(int collection_analysis_id)
        {
            return map.Map<IEnumerable<AnalysisDetailDto>>((await unitOfWork.AnalysisDetailsRepository
                .Query(x=>x.CollectionAnalysisId == collection_analysis_id))
                .OrderByDescending(x=>x.Word));
        }
    }
}
