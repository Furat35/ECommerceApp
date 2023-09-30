namespace ECommerce.Core.Exceptions
{
    public class InternalServerException : Exception
    {
        public InternalServerException(string message = "Internal server error exception!") : base(message)
        {

        }
    }
}
