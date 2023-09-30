using ECommerce.Core.Entities.Concrete;
using ECommerce.Service.Consts;

namespace ECommerce.Entity.Entities
{
    public class Order : BaseEntity
    {
        public Cart Cart { get; set; }
        public float TotalPrice { get; set; }
        public OrderStatusTypes OrderStatusType { get; set; }
    }
}
