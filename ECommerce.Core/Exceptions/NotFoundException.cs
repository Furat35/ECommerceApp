namespace ECommerce.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message = "Not Found Exception!") : base(message)
        {

        }
    }
}
