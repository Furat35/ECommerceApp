namespace ECommerce.Core.Exceptions.AppUser
{
    public class AppUserInternalServerException : InternalServerException
    {
        public AppUserInternalServerException() : base("User internal server exception!")
        {

        }
        public AppUserInternalServerException(string message) : base(message)
        {
        }
    }
}
