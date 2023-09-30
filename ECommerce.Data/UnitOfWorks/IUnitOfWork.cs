using ECommerce.Core.Entities.Concrete;
using ECommerce.Data.Repositories;

namespace ECommerce.Data.UnitOfWorks
{
    public interface IUnitOfWork : IDisposable
    {
        int SaveChanges();
        Task<int> SaveChangesAsync();
        IRepositoryBase<T> GetRepository<T>() where T : BaseEntity, new();
        //EfContext GetContext();
        Task BeginTransactionAsync();
        Task RollbackTransactionAsync();
        Task CommitTransactionAsync();
    }
}
