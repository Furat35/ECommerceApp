using ECommerce.Core.Entities.Concrete;
using ECommerce.Data.Repositories;
using ECommerce.Data.Repositories.EntityFrameworkContext;

namespace ECommerce.Data.UnitOfWorks
{
    public sealed class UnitOfWork : IUnitOfWork
    {
        private readonly EfContext _context;

        public UnitOfWork(EfContext context)
        {
            _context = context;
        }

        public void Dispose()
            => _context.Dispose();


        public IRepositoryBase<T> GetRepository<T>() where T : BaseEntity, new()
            => new RepositoryBase<T>(_context);

        public int SaveChanges()
            => _context.SaveChanges();

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
            => await _context.Database.BeginTransactionAsync();

        public async Task RollbackTransactionAsync()
            => await _context.Database.RollbackTransactionAsync();

        public async Task CommitTransactionAsync()
          => await _context.Database.CommitTransactionAsync();
        //public EfContext GetContext()
        //    => _context;
    }
}
