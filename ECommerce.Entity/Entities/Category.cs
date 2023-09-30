using ECommerce.Core.Entities.Concrete;

namespace ECommerce.Entity.Entities
{
    public class Category : BaseEntity
    {
        public string CategoryName { get; set; }
        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
