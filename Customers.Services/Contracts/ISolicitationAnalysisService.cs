using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface ICollectionAnalysisService
    {        
        Task<List<CollectionAnalysisDto>> GetAnalysesByCollectionId(int collectionId);
        //Task<bool> UpdateReportParameterForAnalysis(AnalysisReportParameterDto reportParameter);
        Task<bool> AddOrUpdateReportParameterForAnalysis(AnalysisReportParameterDto reportParameter);        
        Task<AnalysisReportParameterDto> GetReportParameterByAnalysisId(int collectionAnalysisId);
        Task<vCollectionAnalysisDetails> GetAnalysisViewModel(int collectionAnalysisId);
        Task<AnalysisDocumentCounts> GetAnalysisCount(int collectionAnalysisId);
        Task<int> AddAnalysis(int userId, int sowId, int proposalId, int collectionId,string proposalName);
        Task<ViewAnalysisDocDetails> GetCollectionAnalysisDocDetails(int collectionSOWId, int collectionProposalId);
        Task<KPIResultModel> GetKPIResultForAnalysis(int collectionId, int analysisId);
        Task<int> SaveAnalysis(int collectionAnalysisId, byte[] FileBytes, string FileName);
    }
}
