namespace ECommerce.Core.Exceptions.AppUser
{
    public class AppUserNotFoundException : NotFoundException
    {
        public AppUserNotFoundException() : base("User not found exception!")
        {

        }

        public AppUserNotFoundException(string message) : base(message)
        {

        }
    }
}
