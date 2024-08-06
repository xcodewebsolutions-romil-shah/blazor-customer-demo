using Microsoft.EntityFrameworkCore;
using MySqlX.XDevAPI.Common;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using ZstdSharp.Unsafe;

namespace Customers.Data.Repositories
{
    public class CollectionRepository : GenericRepository<Collection>, ICollectionRepository<Collection>
    {
        private readonly IDbContextFactory<CustomersDBContext> contextFactory;

        public CollectionRepository(IDbContextFactory<CustomersDBContext> contextFactory) : base(contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<IEnumerable<Collection>> GetCollectionsNameAsync(string name, int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var customerId = await context.CustomerUsers.Where(x => x.AspNetUserId == id).Select(x => x.CustomerId).FirstOrDefaultAsync();
                return await context.Collections.Where(x => x.CustomerId == customerId).Where(x => x.Name == name).ToListAsync();
            }
        }

        public async Task<List<Collection>> GetCollectionForUserAsync(int customerId, int userId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var collectionIds = await context.CollectionUsers
                                                        .Where(x => x.UserId == userId)
                                                        .Select(x => x.CollectionId)
                                                        .ToListAsync();

                return await context.Collections
                        .Include(x => x.CollectionSOWs)
                        .Include(x => x.CreatedByUser)
                        .Include(x => x.LastModifiedByUser)
                        .Include(x => x.Customer)
                        .Where(x => x.CustomerId == customerId && collectionIds.Contains(x.CollectionId))
                        .ToListAsync();
            }
        }

        public async Task<List<Collection>> GetCollectionAsync(int customerId, string collectionType)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.Collections
                .Include(x => x.CollectionSOWs)
                .Include(x => x.CreatedByUser)
                .Include(x => x.Customer)
                .Include(x => x.LastModifiedByUser)
                .Where(s => s.CustomerId == customerId &&
                ((collectionType == "All" && !s.IsArchived) ||
                (collectionType == "Open" && s.IsArchived == false && s.IsClosed == false) ||
                (collectionType == "Closed" && s.IsClosed == true))).ToListAsync();
            }
        }

        public async Task<int> GetCustomerIDAsync(int id)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.CustomerUsers
                .Where(x => x.AspNetUserId == id)
                .Select(x => x.CustomerId).FirstOrDefaultAsync();
            }
        }

        public async Task<List<Collection>> GetCollectionWithUsers()
        {
            using (var context = contextFactory.CreateDbContext())
            {
                return await context.Collections
                .Include(x => x.CreatedByUser)
                .Include(x => x.LastModifiedByUser).ToListAsync();
            }
        }

        public async Task<KPIResultModel> GetKPIResult(int collectionId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var lastProposal = await context.CollectionProposals.
                Where(x => x.CollectionId == collectionId && !x.IsArchived).OrderByDescending(x => x.CreatedOn)
                .FirstOrDefaultAsync();

                var lastAnalysis = await context.CollectionAnalyses
                    .Where(x => x.CollectionId == collectionId && x.Status == "active")
                    .OrderByDescending(x => x.RunDateTime).FirstOrDefaultAsync();

                var pageCounts = await context.CollectionProposals.Include(x => x.OriginalDocument)
                    .Where(x => !x.IsArchived && x.CollectionId == collectionId)
                    .Select(x => x.PageCount).ToListAsync();

                double approxPageCount = pageCounts.Any() ? (double)pageCounts.Average() : 0;

                float percentImprovement = 0;
                var TCScore = lastAnalysis == null ? 0 : lastAnalysis.TCScore;
                var lastAnalysisId = lastAnalysis?.CollectionAnalysisId ?? 0;
                var firstAnalysis = await context.CollectionAnalyses.FirstOrDefaultAsync(x => x.RunNumber == 1
                                                                                                                && x.CollectionId == collectionId
                                                                                                                && x.Status == "active"
                                                                                                                && x.CollectionAnalysisId != lastAnalysisId);

                if (firstAnalysis == null || firstAnalysis.CollectionAnalysisId == 0)
                {
                    percentImprovement = 0;
                }
                else
                {
                    try
                    {
                        percentImprovement = (float)(TCScore / firstAnalysis.TCScore) * 100;

                        if (percentImprovement.CompareTo(float.NaN) == 0)
                            percentImprovement = 0;
                    }
                    catch (Exception ex)
                    {
                        percentImprovement = 0;
                    }
                }

                return new KPIResultModel
                {
                    DaysSinceLastProposal = lastProposal == null ? 0 : (DateTime.Now.Date - lastProposal.CreatedOn.Date).Days,
                    ProposalVersion = lastProposal == null ? 0 : lastProposal.VersionNumber,
                    TCScore = TCScore,
                    PercentageImprovement = percentImprovement,
                    ApproximatePageCount = Convert.ToInt32(approxPageCount)
                };
            }
        }
    }
}

