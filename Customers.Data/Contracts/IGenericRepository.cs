using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Customers.Data.Contracts
{
    public interface IGenericRepository<T> : IDisposable where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(int id);
        Task<IEnumerable<T>> Query(Expression<Func<T, bool>> query);
        //Task<List<T>> Query(string query);
        Task<T> AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task DeleteRangeAsync(IEnumerable<T> entities);
        Task<T> QueryFirstOrDefaultAsync(Expression<Func<T, bool>> query);
        //Task<T> QueryFirstOrDefaultAsync<T>(this IQueryable<T> source,Expression<Func<T, bool>> predicate);
    }
}
