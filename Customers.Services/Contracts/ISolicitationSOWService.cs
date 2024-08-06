using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Data.Views;
using Customers.Infrastructure.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface ICollectionSOWService
    {
        Task<List<vCollectionSOWList>> GetDocumentByCollectionId(int id);
        Task<vCollectionSOWList> GetActiveDocument(int collectionId);
        Task DeleteDocument(CollectionSOWDto doc);
        Task<bool> ImportSOW(UploadDocumentModel model);
        Task<bool> SetAsActive(int CollectionId, int CollectionSOWId);
        Task<bool> ArchiveSOW(int collectionSOWId);
    }
}
