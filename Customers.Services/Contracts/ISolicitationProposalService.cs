using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Data.Repositories;
using Customers.Data.Views;
using Customers.Infrastructure.Dtos;
using Customers.Infrastructure.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface ICollectionProposalService
    {
        Task<List<vCollectionProposalList>> GetDocumentByCollectionId(int id);
        Task<Tuple<int, string>> ImportProposal(UploadDocumentModel model);
        Task DeleteDocument(CollectionProposalDto doc);
        Task<bool> SetAsActive(int CollectionId, int CollectionProposalId);
        Task<bool> ArchiveProposal(int collectionProposalId);
    }
}
