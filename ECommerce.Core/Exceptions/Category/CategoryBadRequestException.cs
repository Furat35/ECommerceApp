namespace ECommerce.Core.Exceptions.Category
{
    public class CategoryBadRequestException : BadRequestException
    {
        public CategoryBadRequestException() : base("Category bad request exception!")
        {

        }
        public CategoryBadRequestException(string message) : base(message)
        {
        }
    }
}
