using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface ICollectionProposalRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        Task<bool> SetCollectionProposalAsActive(int collectionProposalId, int collectionId);
        Task<bool> ArchiveProposal(int collectionProposalId);
        Task<bool> SetCollectionProposalAsNotActive(int collectionId);
    }
}
