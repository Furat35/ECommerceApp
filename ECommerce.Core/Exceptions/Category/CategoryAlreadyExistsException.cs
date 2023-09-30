namespace ECommerce.Core.Exceptions.Category
{
    public class CategoryAlreadyExistsException : BadRequestException
    {
        public CategoryAlreadyExistsException(string message) : base(message)
        {

        }

        public CategoryAlreadyExistsException() : base("Category already exists exception!")
        {

        }
    }
}
