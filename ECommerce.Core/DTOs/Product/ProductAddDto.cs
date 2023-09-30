namespace ECommerce.Core.DTOs.Product
{
    public class ProductAddDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public ICollection<string> CategoryIds { get; set; }
    }
}
