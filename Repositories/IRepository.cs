using System.Linq.Expressions;

namespace Server.Repositories;
public interface IRepository<T> where T : class {
    Task<T?> GetAsync(long id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}