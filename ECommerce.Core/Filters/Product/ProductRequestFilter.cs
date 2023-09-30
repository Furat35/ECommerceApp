namespace ECommerce.Core.Filters.Product
{
    public class ProductRequestFilter : Pagination
    {
        //public string? OrderBy { get; set; }
        public string? NameStartsWith { get; set; }
        public float? MinPrice { get; set; }
        public float? MaxPrice { get; set; }

    }
}
