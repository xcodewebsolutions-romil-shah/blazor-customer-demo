using Microsoft.EntityFrameworkCore;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Customers.Data;

namespace Customers.Data.Repositories
{
    public class CollectionAnalysisRepository : GenericRepository<CollectionAnalysis>, ICollectionAnalysisRepository<CollectionAnalysis>
    {
        private readonly IDbContextFactory<CustomersDBContext> contextFactory;

        public CollectionAnalysisRepository(IDbContextFactory<CustomersDBContext> contextFactory) : base(contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<KPIResultModel> GetKPIResultForAnalysis(int collectionId, int analysisId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var result = new KPIResultModel();

                var lastProposal = await context.CollectionProposals.
                    Where(x => x.CollectionId == collectionId && !x.IsArchived).OrderByDescending(x => x.CreatedOn)
                    .FirstOrDefaultAsync();

                result.DaysSinceLastProposal = lastProposal != null ? (DateTime.Now.Date - lastProposal.CreatedOn.Date).Days : 0;

                var lastAnalysis = await context.CollectionAnalyses
                    .Where(x => x.CollectionId == collectionId && x.CollectionAnalysisId == analysisId)
                    .OrderByDescending(x => x.RunDateTime).FirstOrDefaultAsync();

                if (lastAnalysis != null)
                {
                    result.TCScore = lastAnalysis.TCScore;
                    var analysisProposal = await context.CollectionProposals.Include(x => x.OriginalDocument).FirstOrDefaultAsync(x => x.CollectionProposalId == lastAnalysis.ProposalDocumentId);
                    if (analysisProposal != null)
                    {
                        result.ProposalVersion = analysisProposal.VersionNumber;
                        result.ApproximatePageCount = analysisProposal != null ? analysisProposal.PageCount : 0;
                    }
                }

                var firstAnalysis = await context.CollectionAnalyses
                                                .FirstOrDefaultAsync(x => x.RunNumber == 1
                                                                        && x.CollectionId == collectionId
                                                                        && x.Status == "active"
                                                                        && x.CollectionAnalysisId != analysisId);

                if (firstAnalysis == null || firstAnalysis.CollectionAnalysisId == 0)
                {
                    result.PercentageImprovement = 0;
                }
                else
                {
                    try
                    {
                        result.PercentageImprovement = (float)(result.TCScore / firstAnalysis.TCScore) * 100;

                        if (result.PercentageImprovement.CompareTo(float.NaN) == 0)
                            result.PercentageImprovement = 0;
                    }
                    catch (Exception ex)
                    {
                        result.PercentageImprovement = 0;
                    }
                }

                return result;
            }
        }
    }
}
