namespace ECommerce.Core.Filters.Category
{
    public class CategoryRequestFilter : Pagination
    {
        //public string? OrderBy { get; set; }
        public string? Includes { get; set; }
    }
}
