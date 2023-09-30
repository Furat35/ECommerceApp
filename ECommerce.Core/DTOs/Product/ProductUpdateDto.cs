namespace ECommerce.Core.DTOs.Product
{
    public class ProductUpdateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        public ICollection<string> CategoryIds { get; set; } = new List<string>();
    }
}
