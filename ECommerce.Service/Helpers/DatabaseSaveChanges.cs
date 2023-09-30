using ECommerce.Core.Exceptions;
using ECommerce.Data.UnitOfWorks;

namespace ECommerce.Service.Helpers
{
    internal class DatabaseSaveChanges
    {
        private readonly IUnitOfWork _unitOfWork;

        public DatabaseSaveChanges(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        internal async Task<bool> SaveChangesAsync()
        {
            int effectedRows = 0;
            try
            {
                effectedRows = await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new InternalServerException();
            }

            if (effectedRows > 0)
                return true;

            return false;
        }
    }
}
