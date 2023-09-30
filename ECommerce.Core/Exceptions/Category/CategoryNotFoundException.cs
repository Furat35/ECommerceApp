namespace ECommerce.Core.Exceptions.Category
{
    public class CategoryNotFoundException : NotFoundException
    {
        public CategoryNotFoundException() : base("Category not found exception!")
        {

        }

        public CategoryNotFoundException(string message) : base(message)
        {

        }
    }
}
