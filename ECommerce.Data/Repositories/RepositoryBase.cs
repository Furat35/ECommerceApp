using ECommerce.Core.Entities.Concrete;
using ECommerce.Data.Repositories.EntityFrameworkContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ECommerce.Data.Repositories
{
    public sealed class RepositoryBase<T> : IRepositoryBase<T> where T : BaseEntity, new()
    {
        protected readonly EfContext _context;
        public RepositoryBase(EfContext context)
        {
            _context = context;
        }
        DbSet<T> Table => _context.Set<T>();

        public async Task AddAsync(T entity)
            => await _context.AddAsync(entity);

        public async Task AddRangeAsync(List<T> entities)
            => await _context.AddRangeAsync(entities);

        public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
        {
            return predicate != null
                ? await Table.Where(predicate).CountAsync()
                : await Table.CountAsync();
        }

        public async Task<bool> ExistsAsync(T entity)
        {
            T result = await Table.FirstOrDefaultAsync(_ => _.Id == entity.Id);
            return result != null
                ? true
                : false;
        }

        public IQueryable<T> GetAll(Expression<Func<T, bool>> predicate = null, bool trackChanges = false,
            List<Expression<Func<T, object>>> includeProperties = null)
        {
            var query = Table.AsQueryable();
            if (predicate != null)
                query = Table.Where(predicate);
            if (includeProperties != null)
                foreach (var include in includeProperties)
                    query = query.Include(include);
            if (!trackChanges)
                query = query.AsNoTracking();

            return query;
        }

        public async Task<T> GetByIdAsync(string id, List<Expression<Func<T, object>>> includeProperties = null)
        {
            var query = Table.AsQueryable();
            if (includeProperties != null)
                foreach (var include in includeProperties)
                    query = query.Include(include);

            return await query.FirstOrDefaultAsync(_ => _.Id.ToString() == id);
        }

        public async Task<T> GetFirstWhereAsync(Expression<Func<T, bool>> predicate)
            => await Table.Where(predicate).FirstOrDefaultAsync();

        public void SafeRemove(T entity)
         => Table.Update(entity);
        public void SafeRemoveRange(List<T> entities)
            => Table.UpdateRange(entities);

        public void Update(T entity)
            => Table.Update(entity);

        public void UpdateRange(List<T> entities)
            => Table.UpdateRange(entities);
    }
}
