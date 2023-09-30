namespace ECommerce.Core.Exceptions
{
    public class UnprocessableEntity : Exception
    {
        public UnprocessableEntity(string message) : base("Unprocessable entity")
        {

        }
    }
}
