using ECommerce.Core.Entities.Concrete;

namespace ECommerce.Entity.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}
