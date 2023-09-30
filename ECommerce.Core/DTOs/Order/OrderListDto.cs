using ECommerce.Core.DTOs.Cart;

namespace ECommerce.Core.DTOs.Order
{
    public class OrderListDto
    {
        public CartListDto Cart { get; set; }
        public float TotalPrice { get; set; }
        public string OrderStatusType { get; set; }
    }
}
