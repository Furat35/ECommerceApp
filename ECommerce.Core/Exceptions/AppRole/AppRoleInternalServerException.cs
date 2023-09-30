namespace ECommerce.Core.Exceptions.AppRole
{
    public class AppRoleInternalServerException : InternalServerException
    {
        public AppRoleInternalServerException() : base("Role internal server exception!")
        {

        }
        public AppRoleInternalServerException(string message) : base(message)
        {
        }
    }
}
