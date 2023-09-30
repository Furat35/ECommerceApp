namespace ECommerce.Core.Exceptions.Category
{
    public class CategoryAlreadyExists : BadRequestException
    {
        public CategoryAlreadyExists(string message) : base(message)
        {

        }

        public CategoryAlreadyExists() : base("Category already exists exception!")
        {

        }
    }
}
