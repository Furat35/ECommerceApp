namespace ECommerce.Core.Exceptions.AppUser
{
    public class AppUserBadRequestException : BadRequestException
    {
        public AppUserBadRequestException()
        {

        }

        public AppUserBadRequestException(string message) : base(message)
        {

        }
    }
}
