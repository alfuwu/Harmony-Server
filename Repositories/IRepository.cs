using System.Linq.Expressions;

namespace Server.Repositories;
public interface IRepository<T> where T : class {
    Task<T?> GetAsync(long id);
    Task<T?> GetExhaustiveAsync(long id) => GetAsync(id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> GetForIdAsync(long identifyingId) => GetAllAsync();
    Task<T> AddAsync(T entity, bool save = true);
    Task UpdateAsync(T entity, bool save = true);
    Task DeleteAsync(T entity, bool save = true);
    Task SaveAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}