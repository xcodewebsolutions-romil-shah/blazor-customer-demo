using Microsoft.EntityFrameworkCore;
using Customers.Data.Contracts;
using Customers.Data.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Customers.Data;

namespace Customers.Data.Repositories
{
    public class CollectionSOWRepository : GenericRepository<CollectionSOW>, ICollectionSOWRepository<CollectionSOW>
    {
        private readonly IDbContextFactory<CustomersDBContext> contextFactory;

        public CollectionSOWRepository(IDbContextFactory<CustomersDBContext> contextFactory) : base(contextFactory)
        {
            this.contextFactory = contextFactory;
        }

        public async Task<bool> SetCollectionSOWAsActive(int collectionSOWId, int collectionId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var collectionSOWs = await context.CollectionSOWs.Where(x => x.CollectionId == collectionId && x.IsActive == true).ToListAsync();
                collectionSOWs.ForEach(s => s.IsActive = false);
                context.UpdateRange(collectionSOWs);
                await context.SaveChangesAsync();

                var currentCollectionSOW = await context.CollectionSOWs.FindAsync(collectionSOWId);
                if (currentCollectionSOW == null)
                {
                    return false;
                }
                currentCollectionSOW.IsActive = true;
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> SetCollectionSOWAsNotActive(int collectionId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var collectionSOWs = await context.CollectionSOWs.Where(x => x.CollectionId == collectionId && x.IsActive == true).ToListAsync();
                collectionSOWs.ForEach(s => s.IsActive = false);
                context.UpdateRange(collectionSOWs);
                await context.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> ArchiveCollection(int collectionSOWId)
        {
            using (var context = contextFactory.CreateDbContext())
            {
                var collection = await context.CollectionSOWs.FindAsync(collectionSOWId);
                if (collection == null)
                    return false;
                collection.IsArchived = true;
                collection.IsActive = false;
                await context.SaveChangesAsync();
                return true;
            }
        }
    }
}
