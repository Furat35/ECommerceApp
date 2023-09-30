using ECommerce.Core.DTOs.Category;

namespace ECommerce.Core.DTOs.Product
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public ICollection<CategoryListDto> Categories { get; set; } = new List<CategoryListDto>();
    }
}
