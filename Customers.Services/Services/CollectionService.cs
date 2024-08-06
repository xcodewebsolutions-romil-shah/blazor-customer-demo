using AutoMapper;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using Customers.Data.Models;
using Customers.Data.Views;
using Customers.Infrastructure.Dtos;
using Customers.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Services.Services
{
    public class CollectionService(IUnitOfWork unitOfWork, IMapper _mapper) : ICollectionService
    {
        public async Task<IEnumerable<CollectionDto>> GetCollections()
        {
            var collections = await unitOfWork.CollectionRepository.GetCollectionWithUsers();
            return _mapper.Map<IList<Collection>, IList<CollectionDto>>(collections.ToList());
        }

        public async Task<List<CollectionAnalysisDto>> GetCollectionAnalysis(int id)
        {
            var collections = await unitOfWork.CollectionAnalysisRepository.Query(x => x.CollectionId == id);
            return _mapper.Map<IList<CollectionAnalysis>, IList<CollectionAnalysisDto>>(collections.ToList()).ToList();
        }

        public async Task<KPIResultModel> GetKPIResult(int collectionId)
        {
            return await unitOfWork.CollectionRepository.GetKPIResult(collectionId);
        }
        public async Task<AddCollectionDto> GetCollection(int id)
        {
            var collection = _mapper.Map<AddCollectionDto>(await unitOfWork.CollectionRepository.GetByIdAsync(id));
            collection.CollectionUsers = (await unitOfWork.CollectionUsersRepository
                .Query(x => x.CollectionId == id))
                .Select(x => x.UserId)
                .ToList();

            return collection;
        }

        public async Task<vCollectionDashboard> GetDashboardInfo(int id)
        {
            using (var context = unitOfWork.DBContextFactory.CreateDbContext())
            {
                return await context.vCollectionDashboard.FirstOrDefaultAsync(x => x.CollectionId == id) ?? new();
            }
        }

        public async Task UpdateCollection(AddCollectionDto collection)
        {
            await unitOfWork.CollectionRepository.UpdateAsync(_mapper.Map<Collection>(collection));
            var collectionUsers = await unitOfWork.CollectionUsersRepository
                .Query(x => x.CollectionId == collection.CollectionId);

            IEnumerable<int> collectionUsersToAdd = new List<int>();
            List<CollectionUsers> collectionUsersToDelete = new();
            if (collection.CollectionUsers != null && collection.CollectionUsers.Any())
            {
                collectionUsersToAdd = collection.CollectionUsers.
                    Where(x => !collectionUsers.Select(su => su.UserId).Contains(x));
            }

            if (collection.CollectionUsers is null)
                collectionUsersToDelete = collectionUsers.ToList();
            else
                collectionUsersToDelete = collectionUsers
                    .Where(su => !collection.CollectionUsers.Contains(su.UserId)).ToList();

            if (collectionUsersToDelete is not null)
                await unitOfWork.CollectionUsersRepository.DeleteRangeAsync(collectionUsersToDelete);

            await unitOfWork.CollectionUsersRepository.AddRangeAsync(collectionUsersToAdd.Select(x => new CollectionUsers
            {
                CollectionId = collection.CollectionId,
                UserId = x,
                CreatedBy = collection.LastModifiedById ?? 0,
                CreatedOn = DateTime.UtcNow
            }));
        }


        public async Task AddCollection(AddCollectionDto collection)
        {
            var newCollection = await unitOfWork.CollectionRepository.AddAsync(_mapper.Map<Collection>(collection));
            if (collection.CollectionUsers != null && collection.CollectionUsers.Any())
            {
                await unitOfWork.CollectionUsersRepository.AddRangeAsync(collection.CollectionUsers.Select(x => new CollectionUsers
                {
                    UserId = x,
                    CollectionId = newCollection.CollectionId,
                    CreatedBy = newCollection.CreatedById,
                    CreatedOn = DateTime.UtcNow
                }));
            }
        }
        public async Task<bool> DeleteCollection(int id)
        {
            var collection = await unitOfWork.CollectionRepository.GetByIdAsync(id);
            if (collection == null)
            {
                return false;
            }
            await unitOfWork.CollectionRepository.DeleteAsync(collection);
            return true;
        }

        public async Task<bool> GetCollectionsName(string name, int id)
        {
            IEnumerable<Collection> collections = await unitOfWork.CollectionRepository.GetCollectionsNameAsync(name, id);
            if (collections.Count() != 0)
            {
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<CollectionDto>> GetCollectionsAsync(string collectionType, int customerId)
        {
            return _mapper.Map<List<CollectionDto>>(await unitOfWork.CollectionRepository.GetCollectionAsync(customerId, collectionType));
        }

        public async Task<IEnumerable<CollectionDto>> GetCollectionForUserAsync(int customerId, int userId)
        {
            return _mapper.Map<List<CollectionDto>>(await unitOfWork.CollectionRepository.GetCollectionForUserAsync(customerId, userId));
        }

        public async Task<int> GetCustomerId(int id)
        {
            int CustomerID = await unitOfWork.CollectionRepository.GetCustomerIDAsync(id);
            return CustomerID;
        }


    }
}
