using ECommerce.Core.Entities.Concrete;

namespace ECommerce.Entity.Entities
{
    public class Cart : BaseEntity
    {
        public ICollection<CartLine> CartLines { get; set; } = new List<CartLine>();
        public Guid AppUserId { get; set; }
    }
}
