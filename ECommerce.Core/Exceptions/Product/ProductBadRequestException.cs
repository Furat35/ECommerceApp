namespace ECommerce.Core.Exceptions.Product
{
    public class ProductBadRequestException : BadRequestException
    {
        public ProductBadRequestException() : base("Product bad request exception!")
        {

        }

        public ProductBadRequestException(string message) : base(message)
        {

        }
    }
}
