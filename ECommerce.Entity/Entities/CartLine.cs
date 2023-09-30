using ECommerce.Core.Entities.Concrete;

namespace ECommerce.Entity.Entities
{
    public class CartLine : BaseEntity
    {
        public Guid ProductId { get; set; }
        public float QuantityPrice { get; set; }
        public int Quantity { get; set; }
    }
}
