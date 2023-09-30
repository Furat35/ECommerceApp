using ECommerce.Core.Entities.Concrete;
using System.Linq.Expressions;

namespace ECommerce.Data.Repositories
{
    public interface IRepositoryBase<T> where T : BaseEntity, new()
    {
        IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null, bool trackChanges = false, List<Expression<Func<T, object>>> includeProperties = null);
        Task<T> GetByIdAsync(string id, List<Expression<Func<T, object>>> includeProperties = null);
        Task<T> GetFirstWhereAsync(Expression<Func<T, bool>> predicate);
        void SafeRemove(T entity);
        void SafeRemoveRange(List<T> entities);
        Task AddAsync(T entity);
        Task AddRangeAsync(List<T> entities);
        void Update(T entity);
        void UpdateRange(List<T> entities);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
        Task<bool> ExistsAsync(T entity);
    }
}
