using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Data.Views;
using Customers.Infrastructure.Dtos;
using System.Threading.Tasks;

namespace Customers.Services.Contracts
{
    public interface ICollectionService
    {
        Task<IEnumerable<CollectionDto>> GetCollectionsAsync(string collectionType, int customerId);
        Task<IEnumerable<CollectionDto>> GetCollectionForUserAsync(int customerId,int userId);
        Task<IEnumerable<CollectionDto>> GetCollections();
        Task<AddCollectionDto> GetCollection(int id);
        Task<vCollectionDashboard> GetDashboardInfo(int id);
        Task<List<CollectionAnalysisDto>> GetCollectionAnalysis(int id);
        Task AddCollection(AddCollectionDto Collection);
        Task UpdateCollection(AddCollectionDto Collection);
        Task<bool> DeleteCollection(int id);
        Task<bool> GetCollectionsName(string name, int id);
        Task<int> GetCustomerId(int id);
        Task<KPIResultModel> GetKPIResult(int collectionId);
        //Task UpdateAgainCollection(CollectionDto collection);


    }
}