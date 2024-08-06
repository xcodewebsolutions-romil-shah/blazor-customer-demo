using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Customers.Data.Contracts;
using System.Linq.Expressions;

namespace Customers.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly CustomersDBContext _context;
        private readonly IDbContextFactory<CustomersDBContext> _contextFactory;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(IDbContextFactory<CustomersDBContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _context = _contextFactory.CreateDbContext();
            _dbSet = _context.Set<T>();
        }
        public async Task<T> AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> Query(Expression<Func<T, bool>> query)
        {
            return await _dbSet.Where(query).AsNoTracking().ToListAsync();
        }
        public async Task DeleteAsync(T entity)
        {
            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteRangeAsync(IEnumerable<T> entities)
        {
            foreach (var entity in this._context.ChangeTracker.Entries())
            {
                entity.State = EntityState.Detached;
            }
            _dbSet.RemoveRange(entities);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(T entity)
        {
            foreach (var efEntity in this._context.ChangeTracker.Entries())
            {
                efEntity.State = EntityState.Detached;
            }
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<T> GetByIdAsync(int Id)
        {
            var entity = await _dbSet.FindAsync(Id);
            if (entity != null)
            {
                _context.Entry(entity).State = EntityState.Detached;
                return entity;
            }
            return Activator.CreateInstance<T>();
        }

        public async Task<T> QueryFirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
