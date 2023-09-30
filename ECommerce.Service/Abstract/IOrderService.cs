using ECommerce.Core.DTOs.Order;
using ECommerce.Entity.Entities;

namespace ECommerce.Service.Abstract
{
    public interface IOrderService
    {
        Task CompleteOrderAsync(Cart cart);

        Task<List<OrderListDto>> GetAllCompletedOrdersAsync();

        Task<List<OrderListDto>> GetAllNotCompletedOrdersAsync();

        Task<List<OrderListDto>> GetAllCompletedOrdersByUserIdAsync(Guid id);

        Task<List<OrderListDto>> GetAllNotCompletedOrdersByUserIdAsync(Guid id);
    }
}
