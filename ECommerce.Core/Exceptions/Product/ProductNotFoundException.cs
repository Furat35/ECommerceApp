namespace ECommerce.Core.Exceptions.Product
{
    public class ProductNotFoundException : NotFoundException
    {
        public ProductNotFoundException() : base("Product not found exception!")
        {
        }

        public ProductNotFoundException(string message) : base(message)
        {
        }
    }
}
