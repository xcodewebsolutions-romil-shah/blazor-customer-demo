using Microsoft.EntityFrameworkCore;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Repositories
{
    public class CollectionProposalRepository : GenericRepository<CollectionProposal>, ICollectionProposalRepository<CollectionProposal>
    {
        private readonly IDbContextFactory<CustomersDBContext> contextFactory;

        public CollectionProposalRepository(IDbContextFactory<CustomersDBContext> contextFactory) : base(contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<bool> SetCollectionProposalAsActive(int collectionProposalId, int collectionId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var collectionProposals = await context.CollectionProposals.Where(x => x.CollectionId == collectionId && x.IsActive == true).ToListAsync();
                collectionProposals.ForEach(s => s.IsActive = false);
                context.UpdateRange(collectionProposals);
                await context.SaveChangesAsync();

                var currentCollectionProposal = await context.CollectionProposals.FindAsync(collectionProposalId);
                if (currentCollectionProposal == null)
                {
                    return false;
                }
                currentCollectionProposal.IsActive = true;
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> SetCollectionProposalAsNotActive(int collectionId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var collectionProposals = await context.CollectionProposals.Where(x => x.CollectionId == collectionId && x.IsActive == true).ToListAsync();
                collectionProposals.ForEach(s => s.IsActive = false);
                context.UpdateRange(collectionProposals);
                await context.SaveChangesAsync();
                return true;
            }
        }
        public async Task<bool> ArchiveProposal(int collectionProposalId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var proposal = await context.CollectionProposals.FindAsync(collectionProposalId);
                if (proposal == null)
                    return false;
                proposal.IsArchived = true;
                proposal.IsActive = false;
                await context.SaveChangesAsync();

                return true;
            }
        }
    }
}
